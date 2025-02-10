using System;
using System.Collections.Generic;
using System.Linq;

namespace SteamP2PInfo
{
    internal class ConnectionStatistics
    {
        public List<double> ValueList { get; private set; }
        public double Current { get; private set; }
        public double Max => this._max;
        public double Min => this._min;
        public double Avg => this.CalcAvg();
        public double Stdev => this.CalcStdev();

        private double _max;
        private double _min;
        private double _avg;
        private int _avgCalculatedAt;
        private double _stdev;
        private int _stdevCalculatedAt;
        private List<double> _positiveValueList;

        /// <summary>
        /// Constructor
        /// </summary>
        public ConnectionStatistics()
        {
            this.ValueList = new List<double>();
            this._positiveValueList = new List<double>();
            this._avgCalculatedAt = 0;
            this._stdevCalculatedAt = 0;
            this.Current = 0;
            this._max = double.NegativeInfinity;
            this._min = double.PositiveInfinity;
        }

        public void AppendValue(double value)
        {
            this.Current = value;
            this.ValueList.Add(value);

            if (value > this._max) { this._max = value; }
            if (value >= 0)
            {
                this._positiveValueList.Add(value);
                if (value < this._min) { this._min = value; }
            }
        }

        public void Clear()
        {
            this.ValueList.Clear();
            this._positiveValueList.Clear();
            this._avgCalculatedAt = 0;
            this._stdevCalculatedAt = 0;
            this._max = double.NegativeInfinity;
            this._min = double.PositiveInfinity;
        }

        private double CalcAvg()
        {
            int c = this.ValueList.Count;
            if (this._avgCalculatedAt != c)
            {
                this._avgCalculatedAt = c;
                if (this._positiveValueList.Count > 0)
                {
                    this._avg = this._positiveValueList.Average();
                }
            }
            return this._avg;
        }

        private double CalcStdev()
        {
            if (this._positiveValueList.Count == 0)
            {
                return this._stdev;
            }
            int c = this.ValueList.Count;
            if (this._stdevCalculatedAt != c)
            {
                this._stdevCalculatedAt = c;
                double avg = this.Avg;
                double v = this._positiveValueList.Average(item => Math.Pow(item - avg, 2));
                this._stdev = Math.Sqrt(v);
            }
            return this._stdev;
        }
    }
}
