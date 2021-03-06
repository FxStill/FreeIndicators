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

#property indicator_applied_price PRICE_MEDIAN
#property indicator_chart_window

#property indicator_buffers 4
#property indicator_plots   2
//--- plot trendline
#property indicator_label1  "trendline"
#property indicator_type1   DRAW_COLOR_LINE
#property indicator_color1  clrGreen,clrRed,clrLimeGreen
#property indicator_style1  STYLE_SOLID
#property indicator_width1  2

#property indicator_label2  "smoothprice"
#property indicator_type2   DRAW_LINE
#property indicator_color2  clrDodgerBlue
#property indicator_style2  STYLE_SOLID
#property indicator_width2  2

input bool   ShowSmoothPrice = false;
//--- indicator buffers
double         tb[];
double         sb[];
double         tc[];
double         ib[];

static const int MINBAR = 5;
int h;
//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
int OnInit()
  {
   h = iCustom(NULL,0,"EhlersHilbertTransform");
   if (h == INVALID_HANDLE) {
      Print("Error while creating \"EhlersHilbertTransform\"");
      return (INIT_FAILED);
   }  
//--- indicator buffers mapping
   SetIndexBuffer(0,tb,INDICATOR_DATA);
   SetIndexBuffer(1,tc,INDICATOR_COLOR_INDEX);
   SetIndexBuffer(2,sb,INDICATOR_DATA);
   SetIndexBuffer(3,ib,INDICATOR_CALCULATIONS);
   
   ArraySetAsSeries(tb,true);
   ArraySetAsSeries(sb,true);
   ArraySetAsSeries(tc,true);
   ArraySetAsSeries(ib,true);
//---
   IndicatorSetString(INDICATOR_SHORTNAME,"EhlersInstantaneousTrendline");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);
   
   return INIT__SUCCEEDED();
  }
  
void GetValue(const double& price[], int shift, int total) {

   double sp[1];//, sm[1];
   if (CopyBuffer(h, 13, shift, 1, sp) <= 0) return;
//   if (CopyBuffer(h, 2,  shift, 1, sm) <= 0) return;
   
   if (shift <= 1) ib[shift] = 0;
   
   double dcPeriod = MathCeil(sp[0] + 0.5);
   if (dcPeriod <= MINBAR) dcPeriod = MINBAR;
   for (int i = 0; i < dcPeriod; i++) {
      ib[shift] += price[shift + i];
   }

   ib[shift] = ib[shift] / dcPeriod;
   
   tb[shift] = (4 * ib[shift] + 3 * ib[shift + 1] + 2 * ib[shift + 2] + ib[shift + 3]) / 10; 
   double sth = (4 * price[shift] + 3 * price[shift + 1] + 2 * price[shift + 2] + price[shift + 3]) / 10;
   
   if (sth < tb[shift]) tc[shift] = 1 ; 
   else
      if (sth > tb[shift]) tc[shift] = 2 ;   
   
   if (ShowSmoothPrice) sb[shift] = sth;   
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
   if (h != INVALID_HANDLE)
      IndicatorRelease(h);  
}  

//+------------------------------------------------------------------+
//| Custom indicator iteration function                              |
//+------------------------------------------------------------------+
int OnCalculate(const int rates_total,
                const int prev_calculated,
                const int begin,
                const double &price[])
  {
      if(rates_total <= MINBAR) return 0;
      ArraySetAsSeries(price, true);    
      int limit = rates_total - prev_calculated;
      if (limit == 0)        {   // Пришел новый тик 
      } else if (limit == 1) {   // Образовался новый бар
         GetValue(price, 1, rates_total);        
         return(rates_total);       
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(tb,EMPTY_VALUE);
         ArrayInitialize(sb,EMPTY_VALUE);
         ArrayInitialize(tc,0);
         ArrayInitialize(ib,0);
         limit = rates_total - MINBAR;
         for(int i = limit; i >= 1 && !IsStopped(); i--){
            GetValue(price, i, rates_total);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
      GetValue(price, 0, rates_total); 

   return(rates_total);
  }
  
//+------------------------------------------------------------------+
