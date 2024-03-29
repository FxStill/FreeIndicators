//+------------------------------------------------------------------+
//|                                          EhlersRoofingFilter.mq5 |
//|                                Copyright 2022, Andrei Novichkov. |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"
#property description "Ehlers Roofing Filter:\nJohn Ehlers, \"Cycle Analytics for Traders\", pg.80-82"


#property indicator_separate_window

#property indicator_buffers 5


/*********************************************************/
#property indicator_level1 0
#property indicator_levelcolor clrGoldenrod
#property indicator_levelwidth 2
#property indicator_levelstyle STYLE_SOLID

#property indicator_label1  "UpArrow"
#property indicator_type1   DRAW_ARROW
#property indicator_color1  clrBlue
#property indicator_style1  STYLE_SOLID
#property indicator_width1  2

#property indicator_label2  "DwArrow"
#property indicator_type2   DRAW_ARROW
#property indicator_color2  clrRed
#property indicator_style2  STYLE_SOLID
#property indicator_width2  2

/*********************************************************/

#property indicator_label3  "rfilt"
#property indicator_type3   DRAW_LINE
#property indicator_color3  clrLimeGreen
#property indicator_style3  STYLE_SOLID
#property indicator_width3  1
//--- plot trigger
#property indicator_label4  "trigger"
#property indicator_type4   DRAW_LINE
#property indicator_color4  clrDarkBlue
#property indicator_style4  STYLE_SOLID
#property indicator_width4  1

#property indicator_label5  ""
#property indicator_type5   DRAW_NONE

input int      hpLength = 80;
input int      lpLength = 40;
/****************************************************/
input int      iDst     = 100; //Arrow's distance
/****************************************************/


double         rb[];
double         rc[];
double         tb[];
double         hp[];
/************************************/
double         Up[];
double         Dw[];
double         dst;
/************************************/
static const int MINBAR = 5;

double c1, c2, c3;
double a1;
double t1, t2, t3;


//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
int OnInit() {

   SetIndexBuffer(0, Up, INDICATOR_DATA);
   SetIndexBuffer(1, Dw, INDICATOR_DATA);

   SetIndexBuffer(2,rb);
   SetIndexBuffer(3,tb);
   SetIndexBuffer(4,hp);
   
    SetIndexArrow(0,SYMBOL_ARROWUP); 
    SetIndexArrow(1,SYMBOL_ARROWDOWN);    
   
   IndicatorSetString(INDICATOR_SHORTNAME,"EhlersRoofingFilterA");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);   
   

   

   double twoPiPrd = M_SQRT1_2 * 2 * M_PI / hpLength;
   a1 = (MathCos(twoPiPrd) + MathSin(twoPiPrd) - 1) / MathCos(twoPiPrd);
   double a2 = MathExp(-M_SQRT2 * M_PI / lpLength);
   double beta = 2 * a2 * MathCos(M_SQRT2 * M_PI / lpLength);
   c2 = beta;
   c3 = -a2 * a2;
   c1 = 1 - c2 - c3;
   t1 = MathPow(1 - (a1 / 2), 2);
   t2 = 1 - a1;
   t3 = MathPow(t2, 2);
   t2 *= 2;
/********************************/   
   dst = iDst * _Point;
/*******************************/   
   return INIT_SUCCEEDED;
}

void GetValue(const double& price[], int shift) {

   hp[shift] =  t1 * (price[shift] - 2 * price[shift + 1] + price[shift + 2]) +
                t2 * hp[shift + 1] - t3 * hp[shift + 2];

   double r1 = ZerroIfEmpty(rb[shift + 1]);
   double r2 = ZerroIfEmpty(rb[shift + 2]);

   rb[shift] = c1 * ((hp[shift] + hp[shift + 1]) / 2) + c2 * r1 + c3 * r2;
   tb[shift] = r2;
   
   GetArrow(rb, tb, shift);
}

void GetArrow(const double& value[], const double& signal[], int shift) {

   int past  = shift + 1;
   int lpast = past  + 1;
   
   if ((value[past] > signal[past]) && (value[lpast] < signal[lpast]) )
      Up[shift] = value[shift] - dst;
      
   if ((value[past] < signal[past]) && (value[lpast] > signal[lpast]) )
      Dw[shift] = value[shift] + dst;      
}
//+------------------------------------------------------------------+
//|                                                                  |
//+------------------------------------------------------------------+
double ZerroIfEmpty(double value) {
   if (value >= EMPTY_VALUE || value <= -EMPTY_VALUE) return 0.0;
   return value;
}
//+------------------------------------------------------------------+
//|                                                                  |
//+------------------------------------------------------------------+
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
                const long &tick_volume[],
                const long &volume[],
                const int &spread[])
  {
      if(rates_total <= MINBAR) return 0;
      int limit = rates_total - prev_calculated;
      if (limit == 0)        {   // Пришел новый тик 
      } else if (limit == 1) {   // Образовался новый бар
         GetValue(close, 1);  
         return(rates_total);        
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(rb,EMPTY_VALUE);
         ArrayInitialize(tb,EMPTY_VALUE);
         ArrayInitialize(hp,0);
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
