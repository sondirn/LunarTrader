using System;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Alpaca.Markets;
using Microsoft.Extensions.DependencyInjection;
using LunarTrader.Attributes;
using LunarTrader.Interfaces;
using LunarTrader.Services;

namespace LunarTrader.Utils
{
    public static class Commands
    {
        [Command("clear", "clears the terminal")]
        private static void cmdClear()
        {
            Console.Clear();
        }

        [Command("help", "Gets help for a given command")]
        private static void cmdHelp(string command)
        {
            var cmdService = Core.CommandService;
            var logger = Core.ServiceProvider.GetService<ILoggerService>();
            
            if (cmdService.Sorted.Contains(command))
            {
                var cmd = cmdService.Commands[command];

                var str = new StringBuilder();
                
                //Title
                str.Append("(Command: ");
                str.Append(command);
                str.Append(')');
                
                //Usage
                if (!string.IsNullOrEmpty(cmd.Usage))
                {
                    str.Append(' ');
                    str.Append(cmd.Usage);
                }

                logger.LogInfo($"{str.ToString()} - {(string.IsNullOrEmpty(cmd.Help) ? "No help info set" : cmd.Help)}");
                
            }
            else
            {
                var str = new StringBuilder();
                str.Append("Commands list: ");
                str.Append(string.Join(" | ", cmdService.Sorted));
                logger.LogInfo($"{str.ToString()}");
                logger.LogInfo("Type 'help [command name]' for more info on that command");
            }
        }
        
        [Command("getEnumValues", "gets the values of a given enum")]
        private static void CmdGetEnumValues(string enumName)
        {
            var cmdService = Core.ServiceProvider.GetService<ICommandService>();
            try
            {
                var enumType = cmdService?.Enums[enumName];
                var values = Enum.GetNames(enumType ?? throw new Exception("Could not find enum")).ToArray();
                Core.Logger.LogInfo($"{enumName} Values: " + string.Join(" | ", values));
            }
            catch (Exception e)
            {
                Core.Logger.LogError($"Could not fetch enum type with {enumName}: {e.Message}");
            }
            
        }

        [Command("submitOrder", "attempts to request a limit order")]
        private static void cmdSubmitOrder(string symbol, OrderSide orderSide, OrderType orderType, decimal price, int quantity)
        {

            if(orderType == OrderType.Limit)
            {
                Core.ServiceProvider.GetService<ICoreService>().SubmitTask(SubmitLimitOrder(symbol, orderSide, price, quantity));
                
            }
        }

        [Command("deleteOrdersBySymbol", "deletes all orders with a given symbol")]
        private static void cmdDeleteOrdersBySymbol(string symbol)
        {
            Core.ServiceProvider.GetService<ICoreService>().SubmitTask(DeleteOrdersBySymbol(symbol));
        }
        
        [Command("getOrders", "gets all orders on the account")]
        private static void cmdGetOrders()
        {
            Core.ServiceProvider.GetService<ICoreService>().SubmitTask(getOrders());
        }

        [Command("deleteAllPositions", "deletes all positions")]
        private static void cmdDeleteAllPositions()
        {
            Core.ServiceProvider.GetService<ICoreService>().SubmitTask(DeleteAllPositions());
        }

        [Command("getPositions", "gets details on all currently open positions")]
        private static void cmdGetPositions()
        {
            IAlpacaTradingClient client = Core.ServiceProvider.GetService<IAlpacaTradingClient>();
            var positions = client.ListPositionsAsync().Result;

            string resultStr = "\n";

            if(positions.Count == 0)
            {
                Core.Logger.LogInfo("No Active Positions");
                return;
            }

            foreach (IPosition position in positions)
            {
                resultStr += ($"Symbol: {position.Symbol} \n" +
                    $"Avg Entry: {position.AverageEntryPrice} \n" +
                    $"Quantity: {position.Quantity} \n" +
                    $"Side: {position.Side} \n" +
                    $"Market Value: {position.MarketValue} \n" +
                    $"P/L Amount: {position.UnrealizedProfitLoss} \n" +
                    $"P'L Percent: {position.UnrealizedProfitLossPercent} \n" +
                    $"Change Percent: {position.AssetChangePercent} \n" +
                    $"-------------------------------------------------");
            }

            Core.Logger.LogInfo(resultStr);
        }

        [Command("getTime", "gets the current time info, current time, next close & next open")]
        private static void cmdGetTime()
        {
            var client = Core.ServiceProvider.GetService<IAlpacaTradingClient>();
            var clock = client.GetClockAsync().Result;
            var timeService = Core.ServiceProvider.GetService<IDateTimeService>();

            var time = $"\nCurent Time: {timeService.UtcToLocal(clock.TimestampUtc)} \n" +
                $"Next Open: {timeService.UtcToLocal(clock.NextOpenUtc)}\n" +
                $"Next Close: {timeService.UtcToLocal(clock.NextCloseUtc)}\n" +
                $"Market Open: {clock.IsOpen}";

            Core.Logger.LogInfo(time);
        }

        public static async Task DeleteAllPositions()
        {
            var client = Core.ServiceProvider.GetService<IAlpacaTradingClient>();
            var positions = await client.ListPositionsAsync();
            if(positions.Count == 0)
            {
                Core.Logger.LogInfo("No active positions available");
                return;
            }
            try
            {
                foreach (var position in positions)
                {
                    var order = await client.DeletePositionAsync(new DeletePositionRequest(position.Symbol));
                    Core.Logger.LogInfo($"Successfully deleted position with symbol {order.Symbol}");
                }
            }
            catch (Exception e)
            {
                Core.Logger.LogError($"Error during position deletion {e.Message}");
            }
        }

        private static async Task getOrders()
        {
            var coreService = Core.ServiceProvider.GetService<ICoreService>();
            var client = Core.ServiceProvider.GetService<IAlpacaTradingClient>();

            var orders = await client.ListOrdersAsync(new ListOrdersRequest());
            if (orders.Count == 0)
            {
                Core.Logger.LogInfo("No orders are currently open");
                return;
            }

            var resultStr = "$Current Orders: \n" +
                                "\n";
            foreach (var order in orders)
            {
                resultStr +=
                    $"Order ID: {order.OrderId} \n" +
                    $"Symbol: {order.Symbol} \n" +
                    $"Quantity: {order.Quantity} \n" +
                    $"OrderSide: {order.OrderSide} \n" +
                    $"-------------------------------- \n";
            }

            Core.Logger.LogInfo(resultStr);
        }
        private static async Task DeleteOrdersBySymbol(string symbol)
        {
            var client = Core.ServiceProvider.GetService<IAlpacaTradingClient>();
            var orders = client.ListOrdersAsync(new ListOrdersRequest()).Result.Where(x => x.Symbol == symbol).ToList();

            if (orders.Count == 0)
            {
                Core.Logger.LogInfo($"No orders found for symbol: {symbol}");
                return;
            }
            else
            {
                try
                {
                    foreach (var order in orders)
                    {
                        await client.DeleteOrderAsync(order.OrderId);
                        Core.Logger.LogInfo($"Deleted {order.OrderId}");
                    }
                }
                catch (Exception e)
                {
                    Core.Logger.LogError($"Could not cancel order: {e.Message}");
                }

                Core.Logger.LogInfo($"All Orders for {symbol} have been removed");
            }
        }
        private static async Task SubmitLimitOrder(string symbol, OrderSide orderSide, decimal price, int quantity)
        {
            LimitOrder order;
            IAlpacaTradingClient client;
            client = Core.ServiceProvider.GetService<IAlpacaTradingClient>();

            if(orderSide == OrderSide.Buy)
            {
                order = LimitOrder.Buy(symbol, quantity, price);
                try
                {
                    var result = await client.PostOrderAsync(order);
                    Core.Logger.LogInfo($"Order for {result.Symbol} submitted at {result.CreatedAtUtc}");
                }
                catch (Exception e)
                {
                    Core.Logger.LogError($"{symbol} could not be limit ordered: {e.Message}");
                }

            }
            if(orderSide == OrderSide.Sell)
            {
                order = LimitOrder.Sell(symbol, quantity, price);
                try
                {
                    var result = await client.PostOrderAsync(order);
                    Core.Logger.LogInfo($"Order for {result.Symbol} submitted at {result.CreatedAtUtc}");
                }
                catch (Exception e)
                {
                    Core.Logger.LogError($"{symbol} could not be limit ordered: {e.Message}");

                }
            }
        }
    }
}