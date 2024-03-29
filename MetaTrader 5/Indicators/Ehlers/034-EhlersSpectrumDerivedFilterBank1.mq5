//+------------------------------------------------------------------+
//|                              EhlersSpectrumDerivedFilterBank.mq5 |
//|                                Copyright 2020, Andrei Novichkov. |
//|  Main Site: http://fxstill.com                                   |
//|  Telegram:  https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"
#property description "Telegram Channel: https://t.me/fxstill\n"
#property description "The Spectrum Derived Filter Bank:\nJohn Ehlers, \"Stocks & Commodities V. 26:3 (16-22)\""

#property indicator_applied_price PRICE_MEDIAN

#property indicator_separate_window

#property indicator_buffers 1
#property indicator_plots   1
//--- plot V1
#property indicator_label1  "DomCycle"
#property indicator_type1   DRAW_LINE
#property indicator_color1  clrGreen
#property indicator_style1  STYLE_SOLID
#property indicator_width1  2
//--- indicator buffers
double         DomCycle[];
int h;
//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
int OnInit()  {
   h = iCustom(NULL,0,"EhlersBandPassFilter");
   if (h == INVALID_HANDLE) {
      Print("Error while creating \"EhlersBandPassFilter\"");
      return (INIT_FAILED);
   }  
//--- indicator buffers mapping
   SetIndexBuffer(0,DomCycle,INDICATOR_DATA);
   ArraySetAsSeries(DomCycle,true);
//---
   IndicatorSetString(INDICATOR_SHORTNAME,"EhlersSpectrumDerivedFilterBank");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);     
   return INIT__SUCCEEDED();
  }
  
void GetValue(const double& price[], int shift) {
   double dc[1];
   if (CopyBuffer(h, 0, shift, 1, dc) <= 0) {
      return;
   }   
   DomCycle[shift] = dc[0];

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
double MathMedian(double &array[], int start, int count = WHOLE_ARRAY) {
   int size = ArraySize(array);
   if(size < count) return EMPTY_VALUE;
   double sorted_values[];
   if(ArrayCopy(sorted_values, array, 0, start, count) != count)   return EMPTY_VALUE;
   
   ArraySort(sorted_values);
   size = ArraySize(sorted_values);
   if(size % 2 == 1)
      return(sorted_values[size / 2]);
   else
   return (0.5 * (sorted_values[(size - 1) / 2] + sorted_values[(size + 1) / 2]));
}// double MathMedian(double &array[], int start, int count = WHOLE_ARRAY)

  
//+------------------------------------------------------------------+
//| Custom indicator iteration function                              |
//+------------------------------------------------------------------+
int OnCalculate(const int rates_total,
                const int prev_calculated,
                const int begin,
                const double &price[])
  {
      if(rates_total < 4) return 0;
      ArraySetAsSeries(price,true); 
      int limit = rates_total - prev_calculated;
      if (limit == 0)        {   // Пришел новый тик 
      } else if (limit == 1) {   // Образовался новый бар
         GetValue(price, 1);
         return(rates_total);   
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(DomCycle,   EMPTY_VALUE);
         for(int i = limit; i >= 1 && !IsStopped(); i--){
            GetValue(price, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
      GetValue(price, 0);          

   return(rates_total);
  }
  
//+------------------------------------------------------------------+
