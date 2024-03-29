//+------------------------------------------------------------------+
//|                                EhlersTruncatedBandPassFilter.mq5 |
//|                                Copyright 2020, Andrei Novichkov. |
//|  Main Site: http://fxstill.com                                   |
//|  Telegram:  https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"
#property description "Telegram Channel: https://t.me/fxstill\n"
#property description "The Truncated BandPass Filter:\nJohn Ehlers, \"Stocks & Commodities July 2020\", pg.48"


#property indicator_separate_window
#property indicator_buffers 2
//--- plot BP
#property indicator_label1  "BP"
#property indicator_type1   DRAW_LINE
#property indicator_color1  clrDodgerBlue
#property indicator_style1  STYLE_SOLID
#property indicator_width1  1
//--- plot BPT
#property indicator_label2  "BPT"
#property indicator_type2   DRAW_LINE
#property indicator_color2  clrRed
#property indicator_style2  STYLE_SOLID
#property indicator_width2  1
//--- input parameters
input int      Per = 20; //Period
input double   Bandwidth = 0.1;
input int      Length = 10;
//--- indicator buffers
double         bp[];
double         bpt[];

static const int MINBAR = Length + 2;

double l1, g1, s1, s11, s12;
double trunc[];
//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
int OnInit()
  {
//--- indicator buffers mapping
   SetIndexBuffer(0,bp,INDICATOR_DATA);
   SetIndexBuffer(1,bpt,INDICATOR_DATA);
   
   IndicatorSetString(INDICATOR_SHORTNAME,"EhlersTruncatedBandPassFilter");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);
   
	l1  = MathCos(2 * M_PI / Per);
	g1  = MathCos(Bandwidth * 2 * M_PI / Per);
   s1  = 1 / g1 - MathSqrt( (1 / MathPow(g1, 2)) - 1);   
	s11 = 0.5 * (1 - s1);
	s12 = l1 * (1 + s1);   
	
   ArrayResize(trunc, Length + 3);
   ArrayInitialize(trunc, 0);
//---
   return INIT__SUCCEEDED();
  }
  
  
void GetValue(const double& price[], int shift) {

      double b1 = ZerroIfEmpty(bp[shift + 1]);
      double b2 = ZerroIfEmpty(bp[shift + 2]);
      bp[shift] = s11 * (price[shift] - price[shift + 2]) + s12 * b1 - s1 * b2;
		
		for ( int count = Length; count >= 1; count--)
		{
			trunc[count] = s11 * (price[shift + count - 1] - price[shift + count + 1]) +
			               s12 * trunc[count + 1] - s1 * trunc[count + 2];
		}
			
		bpt[shift] = trunc[1];		

} 
int INIT__SUCCEEDED() {
   PlaySound("ok.wav");
   string cm = "Subscribe! https://t.me/fxstill";
   Print(cm);
   Comment("\n"+cm);
   return INIT_SUCCEEDED;
}
double ZerroIfEmpty(double value) {
   if (value >= EMPTY_VALUE || value <= -EMPTY_VALUE) return 0.0;
   return value;
}  
void OnDeinit(const int reason) {
  Comment("");
}
 
//+------------------------------------------------------------------+
//| Custom indicator iteration function                              |
//+------------------------------------------------------------------+
int OnCalculate(const int rates_total,
                const int prev_calculated,
                const datetime &time[],
                const double &open[],
                const double &high[],
                const double &low[],
                const double &close[],
                const long& tick_volume[],
                const long& volume[],
                const int& spread[])
  {
      if(rates_total <= MINBAR) return 0;
      int limit = rates_total - prev_calculated;
      if (limit == 0)        {   // Пришел новый тик 
      } else if (limit == 1) {   // Образовался новый бар
         GetValue(close, 1);  
         return(rates_total);      
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(bp,EMPTY_VALUE);
         ArrayInitialize(bpt,EMPTY_VALUE);
         limit = rates_total - MINBAR;
         for(int i = limit; i >= 1 && !IsStopped(); i--){
            GetValue(close, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
      GetValue(close, 0);  

   return(rates_total);
  }
//+------------------------------------------------------------------+
