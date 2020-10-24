﻿using System;

namespace QuoteManager.Interface
{
    public interface ITradeResult
    {
        Guid Id { get; set; }
        string Symbol { get; set; }
        double VolumeWeightedAveragePrice { get; set; }
        uint VolumeRequested { get; set; }
        uint VolumeExecuted { get; set; }
    }
}
