//+------------------------------------------------------------------+
//|                                      EhlersSinewaveIndicator.mq5 |
//|                                Copyright 2020, Andrei Novichkov. |
//|  Main Site: http://fxstill.com                                   |
//|  Telegram:  https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"
#property description "Telegram Channel: https://t.me/fxstill\n"
#property description "The Sinewave Indicator:\nJohn Ehlers, \"Cybernetic Analysis For Stocks And Futures.\", pg.154-155"

#property indicator_separate_window
#property indicator_applied_price PRICE_MEDIAN

#property indicator_buffers 9
#property indicator_plots   2

#property indicator_type1   DRAW_LINE
#property indicator_width1  1
#property indicator_style1  STYLE_SOLID
#property indicator_color1  clrDodgerBlue

#property indicator_type2   DRAW_LINE
#property indicator_width2  1
#property indicator_style2  STYLE_SOLID
#property indicator_color2  clrRed


static const int MINBAR = 7;

input double alpha1  = 0.07;//Alpha
input int    Length = 5;   //Length

double ha1, ha2, ha22;
int length;
int sz;


double sine[], lsine[];
double smooth[], cycle[], Q1[], I1[], deltaPhase[], InstPeriod[], per[];

int OnInit() {
   SetIndexBuffer(0,sine, INDICATOR_DATA);
   SetIndexBuffer(1,lsine,INDICATOR_DATA);
   
   SetIndexBuffer(2,smooth,INDICATOR_CALCULATIONS);
   SetIndexBuffer(3,cycle,INDICATOR_CALCULATIONS);
   SetIndexBuffer(4,Q1,INDICATOR_CALCULATIONS);
   SetIndexBuffer(5,I1,INDICATOR_CALCULATIONS);
   SetIndexBuffer(6,deltaPhase,INDICATOR_CALCULATIONS);
   SetIndexBuffer(7,InstPeriod,INDICATOR_CALCULATIONS);
   SetIndexBuffer(8,per,INDICATOR_CALCULATIONS);
   
   
   ArraySetAsSeries(sine,true);
   ArraySetAsSeries(lsine,true);      
   ArraySetAsSeries(smooth,true);
   ArraySetAsSeries(cycle,true);
   ArraySetAsSeries(Q1,true);
   ArraySetAsSeries(I1,true);
   ArraySetAsSeries(deltaPhase,true);
   ArraySetAsSeries(InstPeriod,true);
   ArraySetAsSeries(per,true);
   
   IndicatorSetString(INDICATOR_SHORTNAME,"EhlersSinewaveIndicator");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);      
   
   ha1 = MathPow(1 - 0.5 * alpha1, 2);
   ha2 = 1 - alpha1;
   ha22 = MathPow(ha2, 2);
   ha2 *= 2;
   
   length = (Length > 7)? 7: Length;
   sz = 0;
   
   return INIT__SUCCEEDED();
}

void GetValue(const double& price[], int shift) {
   smooth[shift] = (price[shift] + 2 * price[shift + 1] + 2 * price[shift + 2] + price[shift + 3]) / 6;
   cycle[shift]  =  ha1 * (smooth[shift] - 2 * smooth[shift + 1] + smooth[shift + 2]) + 
                    ha2 * cycle[shift + 1] - ha22 * cycle[shift + 2];
   Q1[shift]     = (0.0962 * cycle[shift] + 0.5769 * cycle[shift + 2] - 0.5769 * cycle[shift + 4] -
                    0.0962 * cycle[shift + 6]) * (0.5 + 0.08 * InstPeriod[shift + 1] );  
   I1[shift] = cycle[shift + 3];
   if (Q1[shift] != 0 && Q1[shift + 1] != 0) {
      deltaPhase[shift] = (I1[shift] / Q1[shift] - I1[shift + 1] / Q1[shift + 1]) / 
                          (1 + I1[shift] * I1[shift + 1] / (Q1[shift] * Q1[shift + 1])  );
   } else deltaPhase[shift] = 0;
   if (deltaPhase[shift] < 0.1) deltaPhase[shift] = 0.1;
   if (deltaPhase[shift] > 1.1) deltaPhase[shift] = 1.1;     
   double medianDelta = MathMedian(deltaPhase, shift, length);
   double dc = (medianDelta != 0)? 6.28318 / medianDelta + 0.5: 15;
   InstPeriod[shift] = 0.33 * dc + 0.67 * InstPeriod[shift + 1];
   per[shift] = 0.15 * InstPeriod[shift] + 0.85 * per[shift + 1];
   int dcPeriod = (int)MathCeil(per[shift]);
   double real = 0.0, imag = 0.0;
   for(int i = 0;  i < dcPeriod; i++) { 
      int pos = shift + i;
      if (pos >= sz) continue;
      real += sin(2 * M_PI * i / dcPeriod) * cycle[pos];
      imag += cos(2 * M_PI * i / dcPeriod) * cycle[pos];
   }
   double dcPhase = (MathAbs(imag) > 0.001)? MathArctan(real / imag) * 180 / M_PI: 90 * sign(real);
   dcPhase += 90;
   dcPhase = (imag < 0)? dcPhase + 180: dcPhase;
   dcPhase = (dcPhase > 315)? dcPhase - 360: dcPhase;
   sine[shift]  = MathSin( dcPhase * M_PI / 180);
   lsine[shift] = MathSin((dcPhase + 45) * M_PI / 180);    
}
int sign(double x) {
   if (x == 0.0) return 0;
   if (x >  0.0) return 1;
   return -1;
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

int OnCalculate(const int rates_total,
                const int prev_calculated,
                const int begin,
                const double &price[])  {
                
      if(rates_total < MINBAR) return 0;
      ArraySetAsSeries(price,true);    
      int limit = rates_total - prev_calculated;
      if (limit == 0)        {   // Пришел новый тик 
      } else if (limit == 1) {   // Образовался новый бар
         GetValue(price, 1);
         return(rates_total);      
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(sine,EMPTY_VALUE);
         ArrayInitialize(lsine,EMPTY_VALUE);
         ArrayInitialize(smooth,0);
         ArrayInitialize(cycle,0);
         ArrayInitialize(Q1,0);
         ArrayInitialize(I1,0);
         ArrayInitialize(deltaPhase,0); 
         ArrayInitialize(InstPeriod,0);    
         ArrayInitialize(per,0);            
         limit = rates_total - MINBAR;
         sz = limit;
         for(int i = limit; i >= 1 && !IsStopped(); i--) {
            GetValue(price, i);
         }//for(int i=limit; i>=0 && !IsStopped(); i--)
         return(rates_total);
      }
      GetValue(price, 0);
                
   return(rates_total);
}
