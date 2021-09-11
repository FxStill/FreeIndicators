#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

///+--------------------------------------------------------------------------------------------------+
///|   Site:     https://fxstill.com                                                                  |
///|   Telegram: https://t.me/fxstill (Literature on cryptocurrencies, development and code. )        |
///|                                   Don't forget to subscribe!                                     |
///+--------------------------------------------------------------------------------------------------+

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators.AUN_Indi.Ehlers
{
	public class EhlersFisherStochasticCenterOfGravity : Indicator
	{
		private Series<double> v1;
		private Series<double> v2;
		private Series<double> sg;
		private int MINBAR;
		private double l;
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The Fisher Stochastic Center Of Gravity: John Ehlers";
				Name										= "EhlersFisherStochasticCenterOfGravity";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= false;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				Length					= 8;
				ColorD					= Brushes.Red;
				ColorU					= Brushes.LimeGreen;
				
				AddPlot(Brushes.ForestGreen, "V3");
				AddPlot(Brushes.DodgerBlue, "Trigger");
			}
			else if (State == State.Configure)
			{
				MINBAR = Length + 1;
				l = (Length + 1) / 2;
			}
			else if (State == State.DataLoaded)
			{				
				v1 = new Series<double>(this);
				v2 = new Series<double>(this);
				sg = new Series<double>(this);
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MINBAR) return;
			
   			double num = 0.0;
   			double denom = 0.0;
   			for (int i = 0; i < Length; i++) {
      			num = num + (1 + i) * Median[i];
      			denom = denom + Median[i];   
   			} 
   			if (denom != 0)
      			sg[0] =  l - num / denom;
   			else sg[0] = 0;   

   			double max = MAX(sg, Length)[0];
   			double min = MIN(sg, Length)[0];
			
   			if (max != min) v1[0] = (sg[0] - min) / (max - min);
   			else v1[0] = 0;  			
   			v2[0] = (4 * v1[0] + 3 * v1[1] + 2 * v1[2] + v1[3]) / 10;     
   			V3[0] = 0.5 * Math.Log((1 + (1.98 * (v2[0] - 0.5))) / (1 - (1.98 * (v2[0] - 0.5))));

			Trigger[0] = V3[1];  
			
   			if (V3[0] < Trigger[0]) PlotBrushes[0][0] = ColorD;
   			else
      			if (V3[0] > Trigger[0]) PlotBrushes[0][0] = ColorU;		
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Length", Order=1, GroupName="Parameters")]
		public int Length
		{ get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> V3
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Trigger
		{
			get { return Values[1]; }
		}
		
		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="ColorD", Description="Color for bear", Order=2, GroupName="Parameters")]
		public Brush ColorD
		{ get; set; }

		[Browsable(false)]
		public string ColorDSerializable
		{
			get { return Serialize.BrushToString(ColorD); }
			set { ColorD = Serialize.StringToBrush(value); }
		}			

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="ColorU", Description="Color for bool", Order=3, GroupName="Parameters")]
		public Brush ColorU
		{ get; set; }

		[Browsable(false)]
		public string ColorUSerializable
		{
			get { return Serialize.BrushToString(ColorU); }
			set { ColorU = Serialize.StringToBrush(value); }
		}			
		
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private AUN_Indi.Ehlers.EhlersFisherStochasticCenterOfGravity[] cacheEhlersFisherStochasticCenterOfGravity;
		public AUN_Indi.Ehlers.EhlersFisherStochasticCenterOfGravity EhlersFisherStochasticCenterOfGravity(int length, Brush colorD, Brush colorU)
		{
			return EhlersFisherStochasticCenterOfGravity(Input, length, colorD, colorU);
		}

		public AUN_Indi.Ehlers.EhlersFisherStochasticCenterOfGravity EhlersFisherStochasticCenterOfGravity(ISeries<double> input, int length, Brush colorD, Brush colorU)
		{
			if (cacheEhlersFisherStochasticCenterOfGravity != null)
				for (int idx = 0; idx < cacheEhlersFisherStochasticCenterOfGravity.Length; idx++)
					if (cacheEhlersFisherStochasticCenterOfGravity[idx] != null && cacheEhlersFisherStochasticCenterOfGravity[idx].Length == length && cacheEhlersFisherStochasticCenterOfGravity[idx].ColorD == colorD && cacheEhlersFisherStochasticCenterOfGravity[idx].ColorU == colorU && cacheEhlersFisherStochasticCenterOfGravity[idx].EqualsInput(input))
						return cacheEhlersFisherStochasticCenterOfGravity[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersFisherStochasticCenterOfGravity>(new AUN_Indi.Ehlers.EhlersFisherStochasticCenterOfGravity(){ Length = length, ColorD = colorD, ColorU = colorU }, input, ref cacheEhlersFisherStochasticCenterOfGravity);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersFisherStochasticCenterOfGravity EhlersFisherStochasticCenterOfGravity(int length, Brush colorD, Brush colorU)
		{
			return indicator.EhlersFisherStochasticCenterOfGravity(Input, length, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersFisherStochasticCenterOfGravity EhlersFisherStochasticCenterOfGravity(ISeries<double> input , int length, Brush colorD, Brush colorU)
		{
			return indicator.EhlersFisherStochasticCenterOfGravity(input, length, colorD, colorU);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersFisherStochasticCenterOfGravity EhlersFisherStochasticCenterOfGravity(int length, Brush colorD, Brush colorU)
		{
			return indicator.EhlersFisherStochasticCenterOfGravity(Input, length, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersFisherStochasticCenterOfGravity EhlersFisherStochasticCenterOfGravity(ISeries<double> input , int length, Brush colorD, Brush colorU)
		{
			return indicator.EhlersFisherStochasticCenterOfGravity(input, length, colorD, colorU);
		}
	}
}

#endregion
