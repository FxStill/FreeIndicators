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
	public class EhlersHilbertTransform : Indicator
	{
#region Variables 
//        private int wMAPeriods = 10; // Default setting for WMAPeriods 
		public Series<double> Smooth; 
		private Series<double> Detrender; 
		public Series<double> I1; 
		public Series<double> Q1;		 
		private Series<double> jI; 
		private Series<double> jQ;		 
		private Series<double> I2; 
		private Series<double> Q2; 
		private Series<double> Re; 
		private Series<double> Im;				 
		private Series<double> Period;		 
		public Series<double> SmoothPeriod; 
#endregion 
		
		protected override void OnStateChange()
		{
			switch (State) {
#region	State.SetDefaults			
				case State.SetDefaults:
					Description									= @"HilbertTransform for NT8. Translated from NT7: Andrei Novichkov, http://fxstill.com/contact-page, skype:Andrei.Novichkov.60?call";
					Name										= "EhlersHilbertTransform";
					Calculate									= Calculate.OnBarClose;
					IsOverlay									= false;
					DisplayInDataBox							= true;
					DrawOnPricePanel							= false;
					DrawHorizontalGridLines						= true;
					DrawVerticalGridLines						= true;
					PaintPriceMarkers							= true;
					ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
					IsSuspendedWhileInactive					= true;
					
					
					AddLine(Brushes.Goldenrod, 0, "ZeroLine");
					
					AddPlot(Brushes.Crimson, "InPhase"   );
					AddPlot(Brushes.Green,  "Quadrature");
					
			//					Print(String.Format("{0}: {1}",State.ToString(), ++p));
					break;
#endregion
				case State.Configure:
#region	State.Configure					
					Smooth       = new Series<double>(this); 
					Detrender    = new Series<double>(this); 
					I1           = new Series<double>(this); 
					Q1           = new Series<double>(this); 
					jI           = new Series<double>(this); 
					jQ           = new Series<double>(this); 
					I2           = new Series<double>(this); 
					Q2           = new Series<double>(this); 
					Re           = new Series<double>(this); 
					Im           = new Series<double>(this);			 
					Period       = new Series<double>(this); 
					SmoothPeriod = new Series<double>(this); 					
//					Print(String.Format("{0}: {1}",State.ToString(), ++p));			
				break;
#endregion
							case State.DataLoaded:
			#region	State.DataLoaded								
			//					Print(String.Format("{0}: {1}",State.ToString(), ++p));					
								break;
			#endregion
							case State.Historical:
			#region	State.Historical								
			//					Print(String.Format("{0}: {1}",State.ToString(), ++p));
								break;
			#endregion
							case State.Transition:
			#region	State.Transition								
			//					Print(String.Format("{0}: {1}",State.ToString(), ++p));
								break;
			#endregion
							case State.Realtime:
			#region	State.Realtime								
			//					Print(String.Format("{0}: {1}",State.ToString(), ++p));
								break;
			#endregion
							case State.Terminated:
			#region	State.Terminated								
			//					Print(String.Format("{0}: {1}",State.ToString(), ++p));
								break;
			#endregion
							case State.Active:
			#region	State.Active								
			//					Print(String.Format("{0}: {1}",State.ToString(), ++p));
								break;
			#endregion
							default:
			//					Print(String.Format("{0}: {1}",State.ToString(), ++p));
								break;
						}// switch(State)
						
		}

		protected override void OnBarUpdate()
		{
			if(CurrentBar < 50)  return; 
			//InPhase and Quadrature components	
			Smooth[0] = (4*Median[0] + 3*Median[1] + 2*Median[2] + Median[3])/10; 
			Detrender[0] = (0.0962*Smooth[0]+0.5769*Smooth[2]-0.5769*Smooth[4]-0.0962*Smooth[6])*(0.075*Period[1]+.54); 			
			//Advance the phase of I1 and Q1 by 90 degrees 
			Q1[0] = (0.0962*Detrender[0]+0.5769*Detrender[2]-0.5769*Detrender[4]-0.0962*Detrender[6])*(0.075*Period[1]+0.54);			 
			I1[0] = Detrender[3]; 
			//Phasor Addition 
			I2[0] = I1[0]-jQ[0]; 
			Q2[0] = Q1[0]+jI[0]; 
			//Smooth the I and Q components before applying the discriminator 
			I2[0] = 0.2*I2[0]+0.8*I2[1]; 
			Q2[0] = 0.2*Q2[0]+0.8*Q2[1]; 	
			//Homodyne Discriminator 
			Re[0] = I2[0]*I2[1] + Q2[0]*Q2[1]; 
			Im[0] = I2[0]*Q2[1] - Q2[0]*I2[1]; 
			Re[0] = 0.2*Re[0] + 0.8*Re[1]; 
			Im[0] = 0.2*Im[0] + 0.8*Im[1]; 		
			double rad2Deg = 180.0 / (4.0 * Math.Atan (1)); 
			if(Im[0]!=0 && Re[0]!=0) 
				Period[0] = 360/(Math.Atan(Im[0]/Re[0])*rad2Deg ); 
			 
			if(Period[0]>(1.5*Period[1])) 
				Period[0] = 1.5*Period[1]; 
			 
			if(Period[0]<(0.67*Period[1])) 
				Period[0] = 0.67*Period[1]; 
			 
			if(Period[0]<6) 
				Period[0] = 6; 
			 
			if(Period[0]>50) 
				Period[0] = 50; 
			 
			Period[0] = 0.2*Period[0] + 0.8*Period[1]; 
			SmoothPeriod[0] = 0.33*Period[0] + 0.67*SmoothPeriod[1]; 
			 
			InPhase[0] = I1[0]; 
			Quadrature[0] = Q1[0];			

		}
#region Properties
		[Browsable(false)]	 
        [XmlIgnore()]		 
        public Series<double> InPhase 
        { 
            get { return Values[0]; } 
        } 
 
        [Browsable(false)]	 
        [XmlIgnore()]		
        public Series<double> Quadrature 
        { 
            get { return Values[1]; } 
        } 
		 
		[Browsable(false)] 
		public Series<double> CycleSmoothPeriod{ 
			get{ return SmoothPeriod; } 
		} 
		 
		[Browsable(false)] 
		public Series<double> CyclePeriod{ 
			get{ return Period; } 
		} 

		[Browsable(false)] 
		public Series<double> CycleRe{ 
			get{ return Re; } 
		} 
		
		[Browsable(false)] 
		public Series<double> CycleIm{ 
			get{ return Im; } 
		} 		
#endregion	
	}//public class EhlersHilbertTransform : Indicator
}// namespace NinjaTrader.NinjaScript.Indicators.AUN_Indi

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private AUN_Indi.Ehlers.EhlersHilbertTransform[] cacheEhlersHilbertTransform;
		public AUN_Indi.Ehlers.EhlersHilbertTransform EhlersHilbertTransform()
		{
			return EhlersHilbertTransform(Input);
		}

		public AUN_Indi.Ehlers.EhlersHilbertTransform EhlersHilbertTransform(ISeries<double> input)
		{
			if (cacheEhlersHilbertTransform != null)
				for (int idx = 0; idx < cacheEhlersHilbertTransform.Length; idx++)
					if (cacheEhlersHilbertTransform[idx] != null &&  cacheEhlersHilbertTransform[idx].EqualsInput(input))
						return cacheEhlersHilbertTransform[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersHilbertTransform>(new AUN_Indi.Ehlers.EhlersHilbertTransform(), input, ref cacheEhlersHilbertTransform);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersHilbertTransform EhlersHilbertTransform()
		{
			return indicator.EhlersHilbertTransform(Input);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersHilbertTransform EhlersHilbertTransform(ISeries<double> input )
		{
			return indicator.EhlersHilbertTransform(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersHilbertTransform EhlersHilbertTransform()
		{
			return indicator.EhlersHilbertTransform(Input);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersHilbertTransform EhlersHilbertTransform(ISeries<double> input )
		{
			return indicator.EhlersHilbertTransform(input);
		}
	}
}

#endregion
