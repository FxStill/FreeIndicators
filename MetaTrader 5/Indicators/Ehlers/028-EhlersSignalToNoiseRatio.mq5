//+------------------------------------------------------------------+
//|                                 Ehlers Signal To Noise Ratio.mq5 |
//|                                Copyright 2020, Andrei Novichkov. |
//|  Main Site: http://fxstill.com                                   |
//|  Telegram:  https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"
#property description "Telegram Channel: https://t.me/fxstill\n"
#property description "The Signal To Noise Ratio:\nJohn Ehlers, \"Rocket Science For Traders\", pg.81-82"

#property indicator_separate_window

#property indicator_buffers 3
#property indicator_plots   1

#property indicator_level1     (double)6
#property indicator_levelstyle STYLE_SOLID

#property indicator_label1  "V1"
#property indicator_type1   DRAW_COLOR_LINE 
#property indicator_width1  2
#property indicator_style1  STYLE_SOLID
#property indicator_color1  clrGreen, clrRed, clrLimeGreen 

//#property indicator_label2  "Range"
//#property indicator_type2   DRAW_LINE 
//#property indicator_width2  2
//#property indicator_style2  STYLE_SOLID
//#property indicator_color2  clrDodgerBlue 

double snr[];
double range[];
double ColorBuffer[]; 

static const int MINBAR = 50;

int h;

int OnInit()  {
   h = iCustom(NULL,0,"EhlersHilbertTransform");
   if (h == INVALID_HANDLE) {
      Print("Error while creating \"EhlersHilbertTransform\"");
      return (INIT_FAILED);
   }
   SetIndexBuffer(0,snr,INDICATOR_DATA);
   SetIndexBuffer(1,ColorBuffer,INDICATOR_COLOR_INDEX); 
   SetIndexBuffer(2,range,INDICATOR_CALCULATIONS);  
   
   ArraySetAsSeries(snr,true);
   ArraySetAsSeries(ColorBuffer,true); 
   ArraySetAsSeries(range,true); 
   
   IndicatorSetString(INDICATOR_SHORTNAME,"EhlersSignalToNoiseRatio");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);
   
   return INIT__SUCCEEDED();
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

int OnCalculate(const int rates_total,
                const int prev_calculated,
                const datetime &time[],
                const double &open[],
                const double &high[],
                const double &low[],
                const double &close[],
                const long &tick_volume[],
                const long &volume[],
                const int &spread[]) {
      if(rates_total < MINBAR) return 0;
      ArraySetAsSeries(high,true); 
      ArraySetAsSeries(low,true); 
      int limit = rates_total - prev_calculated;
      if (limit == 0)        {   // Пришел новый тик 
      } else if (limit == 1) {   // Образовался новый бар
         CalcRange(high, low, 1);
         CalcBuff(high, low, 1);      
         return(rates_total);      
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(snr, EMPTY_VALUE);
         ArrayInitialize(range, 0);
         ArrayInitialize(ColorBuffer, 0);
         limit = rates_total - MINBAR;
         for(int i = limit + 1; i >= 1 && !IsStopped(); i--){
            CalcRange(high, low, i);
            CalcBuff(high, low, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);
      }
      CalcRange(high, low, 0);
      CalcBuff(high, low, 0);      
   return(rates_total);
}

void CalcRange(const double& hi[], const double& lo[], int shift) {
   range[shift] = 0.1 * (hi[shift] - lo[shift]) + 0.9 * range[shift + 1];
}// void CalcRange(const double& h[], const double& l[])

double CalcBuff(const double& hi[], const double& lo[], int shift) 
  {
   double I1[1], Q1[1], Smooth[1];
   if (CopyBuffer(h, 2, shift, 1, Smooth) <= 0) return EMPTY_VALUE;
   if (CopyBuffer(h, 4, shift, 1, I1)     <= 0) return EMPTY_VALUE;
   if (CopyBuffer(h, 5, shift, 1, Q1)     <= 0) return EMPTY_VALUE;

   double t = ZerroIfEmpty(snr[shift + 1]);
   
   snr[shift] = (range[shift] > 0) ? 
                (0.25 * ((10 * log(((I1[0] * I1[0]) + (Q1[0] * Q1[0])) / 
                (range[shift] * range[shift])) / log(10)) + 6)) + (0.75 * t) : 0;
               
   t = (hi[shift] + lo[shift]) / 2;             
   if (t < Smooth[0]) ColorBuffer[shift] = 1 ; //red
   else
      if (t > Smooth[0]) ColorBuffer[shift] = 2 ; //blue
     

   return snr[shift];            
  }  

