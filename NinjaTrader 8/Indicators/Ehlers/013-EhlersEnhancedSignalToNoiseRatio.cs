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
	public class EhlersEnhancedSignalToNoiseRatio : Indicator
	{
		private Series<double> q3;
		private Series<double> noise;
		private int MINBAR = 5;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The Enhanced Signal To Noise Ratio: John Ehlers, Rocket Science for Traders., pg.87-88";
				Name										= "EhlersEnhancedSignalToNoiseRatio";
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
				BColor					                    = Brushes.LightGreen;
				SColor					                    = Brushes.LightCoral;				
				AddPlot(Brushes.Gray, "Snr");
				AddLine(Brushes.SkyBlue, 6, "LevelLine");
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				q3 = new Series<double>(this);
				noise = new Series<double>(this);
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MINBAR) return;
			double pr = EhlersHilbertTransform(Input).SmoothPeriod[0];
			double smooth = EhlersHilbertTransform(Input).Smooth[0];
			double hl2 = (High[0] + Low[0]) / 2;
			q3[0] = 0.5 * (EhlersHilbertTransform(Input).Smooth[0] - EhlersHilbertTransform(Input).Smooth[2]) * (0.1759 * pr + 0.4607);
   			double i3 = 0.0;
   			int sp = (int)Math.Ceiling(pr / 2);
   			if (sp == 0) sp = 1;
			
		    for (int i = 0; i < sp; i++) {
       			i3 += q3[i];
   			}    
   			i3 = (1.57 * i3) / sp;
   
   			double signal = Math.Pow(i3, 2) + Math.Pow(q3[0], 2);			

   			noise[0] = 0.1 * Math.Pow((High[0] - Low[0]), 2) * 0.25 + 0.9 * noise[1];
   
   			if (noise[0] != 0.0 && signal != 0) {
//      double s = (snr[shift + 1] == EMPTY_VALUE)? 0: snr[shift + 1];
//			      double s = ZerroIfEmpty(snr[shift + 1]);
      		Snr[0] = 0.33 * 10 * Math.Log(signal / noise[0]) / Math.Log(10) + 0.67 * Snr[1];
		   } else {
      			Snr[0] = 0;
		   }
		   if (hl2 > smooth) {
		   		PlotBrushes[0][0] = BColor;
		   } else 
			   if (hl2 < smooth) {
			   		PlotBrushes[0][0] = SColor;
			   }
   		}

		#region Properties

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Snr
		{
			get { return Values[0]; }
		}
		
		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="BColor", Description="Buy Color", Order=2, GroupName="Parameters")]
		public Brush BColor
		{ get; set; }

		[Browsable(false)]
		public string BColorSerializable
		{
			get { return Serialize.BrushToString(BColor); }
			set { BColor = Serialize.StringToBrush(value); }
		}			

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="SColor", Description="Sell Color", Order=3, GroupName="Parameters")]
		public Brush SColor
		{ get; set; }

		[Browsable(false)]
		public string SColorSerializable
		{
			get { return Serialize.BrushToString(SColor); }
			set { SColor = Serialize.StringToBrush(value); }
		}			
		
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private AUN_Indi.Ehlers.EhlersEnhancedSignalToNoiseRatio[] cacheEhlersEnhancedSignalToNoiseRatio;
		public AUN_Indi.Ehlers.EhlersEnhancedSignalToNoiseRatio EhlersEnhancedSignalToNoiseRatio(Brush bColor, Brush sColor)
		{
			return EhlersEnhancedSignalToNoiseRatio(Input, bColor, sColor);
		}

		public AUN_Indi.Ehlers.EhlersEnhancedSignalToNoiseRatio EhlersEnhancedSignalToNoiseRatio(ISeries<double> input, Brush bColor, Brush sColor)
		{
			if (cacheEhlersEnhancedSignalToNoiseRatio != null)
				for (int idx = 0; idx < cacheEhlersEnhancedSignalToNoiseRatio.Length; idx++)
					if (cacheEhlersEnhancedSignalToNoiseRatio[idx] != null && cacheEhlersEnhancedSignalToNoiseRatio[idx].BColor == bColor && cacheEhlersEnhancedSignalToNoiseRatio[idx].SColor == sColor && cacheEhlersEnhancedSignalToNoiseRatio[idx].EqualsInput(input))
						return cacheEhlersEnhancedSignalToNoiseRatio[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersEnhancedSignalToNoiseRatio>(new AUN_Indi.Ehlers.EhlersEnhancedSignalToNoiseRatio(){ BColor = bColor, SColor = sColor }, input, ref cacheEhlersEnhancedSignalToNoiseRatio);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersEnhancedSignalToNoiseRatio EhlersEnhancedSignalToNoiseRatio(Brush bColor, Brush sColor)
		{
			return indicator.EhlersEnhancedSignalToNoiseRatio(Input, bColor, sColor);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersEnhancedSignalToNoiseRatio EhlersEnhancedSignalToNoiseRatio(ISeries<double> input , Brush bColor, Brush sColor)
		{
			return indicator.EhlersEnhancedSignalToNoiseRatio(input, bColor, sColor);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersEnhancedSignalToNoiseRatio EhlersEnhancedSignalToNoiseRatio(Brush bColor, Brush sColor)
		{
			return indicator.EhlersEnhancedSignalToNoiseRatio(Input, bColor, sColor);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersEnhancedSignalToNoiseRatio EhlersEnhancedSignalToNoiseRatio(ISeries<double> input , Brush bColor, Brush sColor)
		{
			return indicator.EhlersEnhancedSignalToNoiseRatio(input, bColor, sColor);
		}
	}
}

#endregion
