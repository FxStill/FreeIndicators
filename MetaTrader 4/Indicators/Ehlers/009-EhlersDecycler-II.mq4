//+------------------------------------------------------------------+
//|                                            EhlersDecycler-II.mq4 |
//|                                Copyright 2021, Andrei Novichkov. |
//|  Main Site: http://fxstill.com                                   |
//|  Telegram:  https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"
#property description "Telegram Channel: https://t.me/fxstill\n"
#property description "The Decycler:\nJohn Ehlers, \"Stocks & Commodities V. 33:09\", pg.12-15"

#property indicator_chart_window
#property indicator_buffers 4
//--- plot decycler
#property indicator_label1  "decycler"
#property indicator_type1   DRAW_LINE
#property indicator_color1  clrBlue
#property indicator_style1  STYLE_SOLID
#property indicator_width1  2
//--- plot decyclerUp
#property indicator_label2  "decyclerUp"
#property indicator_type2   DRAW_LINE
#property indicator_color2  clrLimeGreen
#property indicator_style2  STYLE_SOLID
#property indicator_width2  1
//--- plot decyclerDown
#property indicator_label3  "decyclerDown"
#property indicator_type3   DRAW_LINE
#property indicator_color3  clrLimeGreen
#property indicator_style3  STYLE_SOLID
#property indicator_width3  1

input int      length = 20;   //Period
input double   wdt    = 0.2; //Channel Width

//--- indicator buffers
double         decyclerBuffer[];
double         decyclerUpBuffer[];
double         decyclerDownBuffer[];
double         hp[];

static const int MINBAR = 5;

double a1, a2, a3, wu, wd;
//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
int OnInit()
  {
//--- indicator buffers mapping
   SetIndexBuffer(0,decyclerBuffer,INDICATOR_DATA);
   SetIndexBuffer(1,decyclerUpBuffer,INDICATOR_DATA);
   SetIndexBuffer(2,decyclerDownBuffer,INDICATOR_DATA);
   SetIndexBuffer(3,hp,INDICATOR_CALCULATIONS);
   
   IndicatorSetString(INDICATOR_SHORTNAME,"EhlersDecycler-II");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);   
   
   double t0 = ((.707 * 360 / length) * M_PI) / 180;
   double tc = MathCos(t0);
   double ts = MathSin(t0);
   double alpha = (tc + ts - 1) / tc;
   
	a1  = 1 - alpha;
	a2  = 1 - alpha / 2;
	a2 *= a2;
	a3  = a1 * a1;
	wu  = 1.0 + wdt / 200.0;
	wd  = 1.0 - wdt / 200.0; 
   
   return INIT__SUCCEEDED();
  }
  
  
void GetValue(const double& price[], int shift) {

   double d1 = ZerroIfEmpty(hp[shift + 1]);
   double d2 = ZerroIfEmpty(hp[shift + 2]);
   
   hp[shift] = a2 * (price[shift] - 2 * price[shift + 1] + price[shift + 2]) + 2 * a1 * d1 - a3 * d2;
   
   decyclerBuffer[shift]     = price[shift] - hp[shift];
	decyclerUpBuffer[shift]   = wu * decyclerBuffer[shift];
	decyclerDownBuffer[shift] = wd * decyclerBuffer[shift];   
	
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
//---
      if(rates_total <= MINBAR) return 0;

      int limit = rates_total - prev_calculated;
      
      if (limit == 0)        {   
      
      } else if (limit == 1) {  
      
         GetValue(close, 1);  
         
         return(rates_total);                  
      } else if (limit > 1)  {   
      
         ArrayInitialize(decyclerBuffer,EMPTY_VALUE);
         ArrayInitialize(decyclerUpBuffer,EMPTY_VALUE);
         ArrayInitialize(decyclerDownBuffer,EMPTY_VALUE);
         ArrayInitialize(hp,0);
         
         limit = rates_total - MINBAR;
         for(int i = limit; i >= 1 && !IsStopped(); i--){
            GetValue(close, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         
         return(rates_total);         
      }
      GetValue(close, 0); 
   
//--- return value of prev_calculated for next call
   return(rates_total);
  }
//+------------------------------------------------------------------+
