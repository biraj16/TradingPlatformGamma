// TradingConsole.Wpf/Services/Analysis/AnalysisStateManager.cs
using System;
using System.Collections.Generic;
using TradingConsole.Core.Models;
using TradingConsole.Wpf.ViewModels;

namespace TradingConsole.Wpf.Services.Analysis
{
    public class AnalysisStateManager
    {
        public MarketPhase CurrentMarketPhase { get; set; } = MarketPhase.PreOpen;

        public Dictionary<string, AnalysisResult> AnalysisResults { get; } = new();
        public Dictionary<string, MarketProfile> MarketProfiles { get; } = new();
        public Dictionary<string, List<MarketProfileData>> HistoricalMarketProfiles { get; } = new();

        public HashSet<string> BackfilledInstruments { get; } = new();
        public Dictionary<string, (decimal cumulativePriceVolume, long cumulativeVolume, List<decimal> ivHistory)> TickAnalysisState { get; } = new();

        public Dictionary<string, Dictionary<TimeSpan, List<Candle>>> MultiTimeframeCandles { get; } = new();
        public Dictionary<string, Dictionary<TimeSpan, EmaState>> MultiTimeframePriceEmaState { get; } = new();
        public Dictionary<string, Dictionary<TimeSpan, EmaState>> MultiTimeframeVwapEmaState { get; } = new();
        public Dictionary<string, Dictionary<TimeSpan, RsiState>> MultiTimeframeRsiState { get; } = new();
        public Dictionary<string, Dictionary<TimeSpan, AtrState>> MultiTimeframeAtrState { get; } = new();
        public Dictionary<string, Dictionary<TimeSpan, ObvState>> MultiTimeframeObvState { get; } = new();

        public Dictionary<string, IntradayIvState> IntradayIvStates { get; } = new();
        public Dictionary<string, IntradayIvState.CustomLevelState> CustomLevelStates { get; } = new();
        public Dictionary<string, (bool isBreakout, bool isBreakdown)> InitialBalanceState { get; } = new();

        public Dictionary<string, RelativeStrengthState> RelativeStrengthStates { get; } = new();
        public Dictionary<string, IvSkewState> IvSkewStates { get; } = new();
        public Dictionary<string, DateTime> LastSignalTime { get; } = new();

        public Dictionary<string, bool> IsInVolatilitySqueeze { get; } = new();


        private readonly List<TimeSpan> _timeframes = new()
        {
            TimeSpan.FromMinutes(1),
            TimeSpan.FromMinutes(3),
            TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(15)
        };

        public void InitializeStateForInstrument(DashboardInstrument instrument)
        {
            if (BackfilledInstruments.Contains(instrument.SecurityId)) return;

            BackfilledInstruments.Add(instrument.SecurityId);
            AnalysisResults[instrument.SecurityId] = new AnalysisResult
            {
                SecurityId = instrument.SecurityId,
                Symbol = instrument.DisplayName,
                InstrumentGroup = instrument.InstrumentType,
                UnderlyingGroup = instrument.UnderlyingSymbol,
                StrikePrice = instrument.StrikePrice,
                OptionType = instrument.OptionType
            };
            TickAnalysisState[instrument.SecurityId] = (0, 0, new List<decimal>());
            MultiTimeframeCandles[instrument.SecurityId] = new Dictionary<TimeSpan, List<Candle>>();
            MultiTimeframePriceEmaState[instrument.SecurityId] = new Dictionary<TimeSpan, EmaState>();
            MultiTimeframeVwapEmaState[instrument.SecurityId] = new Dictionary<TimeSpan, EmaState>();
            MultiTimeframeRsiState[instrument.SecurityId] = new Dictionary<TimeSpan, RsiState>();
            MultiTimeframeAtrState[instrument.SecurityId] = new Dictionary<TimeSpan, AtrState>();
            MultiTimeframeObvState[instrument.SecurityId] = new Dictionary<TimeSpan, ObvState>();
            IsInVolatilitySqueeze[instrument.SecurityId] = false;

            if (instrument.InstrumentType == "INDEX")
            {
                RelativeStrengthStates[instrument.SecurityId] = new RelativeStrengthState();
                IvSkewStates[instrument.SecurityId] = new IvSkewState();
                CustomLevelStates[instrument.Symbol] = new IntradayIvState.CustomLevelState();
            }

            foreach (var tf in _timeframes)
            {
                MultiTimeframeCandles[instrument.SecurityId][tf] = new List<Candle>();
                MultiTimeframePriceEmaState[instrument.SecurityId][tf] = new EmaState();
                MultiTimeframeVwapEmaState[instrument.SecurityId][tf] = new EmaState();
                MultiTimeframeRsiState[instrument.SecurityId][tf] = new RsiState();
                MultiTimeframeAtrState[instrument.SecurityId][tf] = new AtrState();
                MultiTimeframeObvState[instrument.SecurityId][tf] = new ObvState();
            }
        }

        public List<Candle>? GetCandles(string securityId, TimeSpan timeframe)
        {
            if (MultiTimeframeCandles.TryGetValue(securityId, out var timeframes) &&
                timeframes.TryGetValue(timeframe, out var candles))
            {
                return candles;
            }
            return null;
        }

        public AnalysisResult GetResult(string securityId)
        {
            if (!AnalysisResults.ContainsKey(securityId))
            {
                AnalysisResults[securityId] = new AnalysisResult { SecurityId = securityId };
            }
            return AnalysisResults[securityId];
        }
    }
}
