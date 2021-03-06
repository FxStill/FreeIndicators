//+------------------------------------------------------------------+
//|                                 EhlersInstantaneousTrendline.mq5 |
//|                                Copyright 2020, Andrei Novichkov. |
//|  Main Site: http://fxstill.com                                   |
//|  Telegram:  https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"
#property description "Telegram Channel: https://t.me/fxstill\n"
#property description "The Instantaneous Trendline:\nJohn Ehlers, \"Rocket Science For Traders\", pg.109-110"


#property indicator_chart_window

#property indicator_buffers  3
//--- plot trendline
#property indicator_label1  "trendline"
#property indicator_type1   DRAW_LINE
#property indicator_color1  clrGreen
#property indicator_style1  STYLE_SOLID
#property indicator_width1  2

#property indicator_label2  "smoothprice"
#property indicator_type2   DRAW_LINE
#property indicator_color2  clrRed
#property indicator_style2  STYLE_SOLID
#property indicator_width2  2

#property indicator_label3  ""
#property indicator_type3   DRAW_NONE
//--- indicator buffers
double         tb[];
double         sm[];
double         ib[];

static const int MINBAR = 5;
//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
int OnInit() {
   SetIndexBuffer(0,tb);
   SetIndexBuffer(1,sm);
   SetIndexBuffer(2,ib);

   IndicatorSetString(INDICATOR_SHORTNAME,"EhlersInstantaneousTrendline");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);
   
   return INIT__SUCCEEDED();
}
  
void GetValue(const double& h[], const double& l[], int shift, int total) {

   double SmoothPeriod = iCustom(NULL,0,"EhlersHilbertTransform",13,shift);
   double Smooth       = iCustom(NULL,0,"EhlersHilbertTransform",2, shift);
   
   double price0 = (h[shift] + l[shift]) / 2;
   double price1 = (h[shift + 1] + l[shift + 1]) / 2;
   double price2 = (h[shift + 2] + l[shift + 2]) / 2;
   double price3 = (h[shift + 3] + l[shift + 3]) / 2;
      
   if (shift <= 1) ib[shift] = 0;
   
   double dcPeriod = MathCeil(SmoothPeriod + 0.5);
   if (dcPeriod <= MINBAR) dcPeriod = MINBAR;
   
   for (int i = 0; i < dcPeriod; i++) {
      double p = (h[shift + i] + l[shift + i]) / 2;
      ib[shift] += p;
   }
   if (dcPeriod > 0) ib[shift] = ib[shift] / dcPeriod;
   
   tb[shift] = (4 * ib[shift] + 3 * ib[shift + 1] + 2 * ib[shift + 2] + ib[shift + 3]) / 10;  
   
   sm[shift] = (4 * price0 + 3 * price1 + 2 * price2 + price3) / 10;
   
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
         GetValue(high, low, 1, rates_total);        
         return(rates_total); 
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(tb,EMPTY_VALUE);
         ArrayInitialize(sm,EMPTY_VALUE);
         ArrayInitialize(ib,0);
         limit = rates_total - MINBAR;
         for(int i = limit; i >= 1 && !IsStopped(); i--){
            GetValue(high, low, i, rates_total);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
      GetValue(high, low, 0, rates_total); 

   return(rates_total);
  }
