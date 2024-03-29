//+------------------------------------------------------------------+
//|                                EhlersPredictiveMovingAverage.mq5 |
//|                                Copyright 2020, Andrei Novichkov. |
//|  Main Site: http://fxstill.com                                   |
//|  Telegram:  https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"
#property description "Telegram Channel: https://t.me/fxstill\n"
#property description "The Predictive Moving Average:\nJohn Ehlers, \"Rocket Science For Traders\", pg.212"

#property indicator_chart_window

#property indicator_buffers 4
//--- plot predict
#property indicator_label1  "predict"
#property indicator_type1   DRAW_LINE
#property indicator_color1  clrDarkBlue
#property indicator_style1  STYLE_SOLID
#property indicator_width1  2
//--- plot trigger
#property indicator_label2  "trigger"
#property indicator_type2   DRAW_LINE
#property indicator_color2  clrLimeGreen
#property indicator_style2  STYLE_SOLID
#property indicator_width2  2

#property indicator_label3  ""
#property indicator_type3   DRAW_NONE

#property indicator_label4  ""
#property indicator_type4   DRAW_NONE
//--- indicator buffers
double         pb[];
double         tr[];
double         wma1[], wma2[];
static const int MINBAR = 13;
//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
int OnInit()
  {
//--- indicator buffers mapping
   SetIndexBuffer(0,pb);
   SetIndexBuffer(1,tr);
   SetIndexBuffer(2,wma1);
   SetIndexBuffer(3,wma2);

   IndicatorSetString(INDICATOR_SHORTNAME,"EhlersPredictiveMovingAverage");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);      

   return INIT__SUCCEEDED();
  }

void GetValue(const double& h[], const double& l[], int shift) {

   double price0 = (h[shift] + l[shift]) / 2;
   double price1 = (h[shift + 1] + l[shift + 1]) / 2;
   double price2 = (h[shift + 2] + l[shift + 2]) / 2;
   double price3 = (h[shift + 3] + l[shift + 3]) / 2;
   double price4 = (h[shift + 4] + l[shift + 4]) / 2;
   double price5 = (h[shift + 5] + l[shift + 5]) / 2;
   double price6 = (h[shift + 6] + l[shift + 6]) / 2;
      
   wma1[shift] = (7 * price0 + 6 * price1 + 5 * price2 + 4 * price3 + 3 * price4 + 2 * price5 + price6) / 28;
   wma2[shift] = (7 * wma1[shift] +  6 * wma1[shift + 1] +  5 * wma1[shift + 2] +  4 * wma1[shift + 3] +  3 * wma1[shift + 4] +  2 * wma1[shift + 5] +  wma1[shift + 6])  / 28;
   
   tr[shift] = 2 * wma1[shift] - wma2[shift];
   pb[shift] = (4 * tr[shift] + 3 * tr[shift + 1] + 2 * tr[shift + 2] + tr[shift + 3]) / 10;   
   
      
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
         GetValue(high, low, 1);  
         return(rates_total);      
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(pb,EMPTY_VALUE);
         ArrayInitialize(tr,EMPTY_VALUE);
         ArrayInitialize(wma1,0);
         ArrayInitialize(wma2,0);
         limit = rates_total - MINBAR;
         for(int i = limit; i >= 1 && !IsStopped(); i--){
            GetValue(high, low, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
      GetValue(high, low, 0);  

   return(rates_total);
  }
//+------------------------------------------------------------------+
