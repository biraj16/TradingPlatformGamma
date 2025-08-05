// In TradingConsole.Core/Models/DashboardInstrument.cs
using System;
using TradingConsole.DhanApi.Models;

namespace TradingConsole.Core.Models
{
    public class DashboardInstrument : LiveInstrumentData
    {
        public int SegmentId { get; set; }
        public string ExchId { get; set; } = string.Empty;
        public bool IsFuture { get; set; }
        public string UnderlyingSymbol { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;

        public string InstrumentType { get; set; } = string.Empty;

        public decimal StrikePrice { get; set; }
        public string OptionType { get; set; } = string.Empty;
        public DateTime? ExpiryDate { get; set; }

        public Greeks? Greeks { get; set; }


        private long _openInterest;
        public long OpenInterest
        {
            get => _openInterest;
            set { if (_openInterest != value) { _openInterest = value; OnPropertyChanged(); } }
        }

        private decimal _impliedVolatility;
        public decimal ImpliedVolatility
        {
            get => _impliedVolatility;
            set { if (_impliedVolatility != value) { _impliedVolatility = value; OnPropertyChanged(); } }
        }

        private string? _tradingSignal;
        public string? TradingSignal
        {
            get => _tradingSignal;
            set { if (_tradingSignal != value) { _tradingSignal = value; OnPropertyChanged(); } }
        }
    }
}
