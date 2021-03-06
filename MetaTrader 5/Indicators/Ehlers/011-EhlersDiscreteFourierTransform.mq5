//+------------------------------------------------------------------+
//|                               EhlersDiscreteFourierTransform.mq5 |
//|                                Copyright 2020, Andrei Novichkov. |
//|  Main Site: http://fxstill.com                                   |
//|  Telegram:  https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"
#property description "Telegram Channel: https://t.me/fxstill\n"
#property description "The Discrete Fourier Transform Indicator, https://www.mesasoftware.com/papers/FourierTransformForTraders.pdf"

#property indicator_applied_price PRICE_MEDIAN

#property indicator_separate_window

#property indicator_buffers 3
#property indicator_plots   1
//--- plot V1
#property indicator_label1  "DomCycle"
#property indicator_type1   DRAW_LINE
#property indicator_color1  clrGreen
#property indicator_style1  STYLE_SOLID
#property indicator_width1  2
//--- input parameters
input int      minperiod     = 8;    //MinPeriod
input int      maxperiod     = 50;   //MaxPeriod
input int      hpPeriod      = 40;   //HpPeriod
input int      medianPeriod  = 10;   //MedianPeriod 
input int      decibelPeriod = 20;   //DecibelPeriod
input int      iMaxBar       = 500;  //Max bars for picture
input int      iWidth        = 20;   //Lines Width
input color    cSpectr       = clrGold; //Spectr Color

int red, green, blue;

//--- indicator buffers 
double         DomCycle[];
double cleanedData[], hp[];

double Pwr[];
double DB[];

string NAME = "EhlersDiscreteFourierTransform";

#define ARGB(a,r,g,b)  ((uchar(a)<<24)|(uchar(r)<<16)|(uchar(g)<<8)|uchar(b))
#define GETRGBR(clr)   uchar((clr)>>16)
#define GETRGBG(clr)   uchar((clr)>>8)
#define GETRGBB(clr)   uchar(clr)

static const int MINBAR = maxperiod + 1;
double a1, a2;
int sub_win;
//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
int OnInit()
  {
//--- indicator buffers mapping
   SetIndexBuffer(0,DomCycle,INDICATOR_DATA);
   SetIndexBuffer(1,cleanedData,INDICATOR_CALCULATIONS);
   SetIndexBuffer(2,hp,INDICATOR_CALCULATIONS);
   
   ArraySetAsSeries(DomCycle,true);
   ArraySetAsSeries(cleanedData,true); 
   ArraySetAsSeries(hp,true); 
//---

   
   IndicatorSetString(INDICATOR_SHORTNAME,NAME);
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);     

   a1 = (1 - MathSin(2 * M_PI / hpPeriod)) / MathCos(2 * M_PI / hpPeriod);
   a2 = 0.5 * (1 + a1);
   
   InitArrays();
   
   sub_win = -1;
   red   = GETRGBR(cSpectr);
   green = GETRGBG(cSpectr);
   blue  = GETRGBB(cSpectr);
      
   return INIT__SUCCEEDED();
  }
  
void GetValue(const double& price[], int shift) {

   hp[shift] = a2 * (price[shift] - price[shift + 1]) + a1 * hp[shift + 1];
   cleanedData[shift] = (hp[shift] + 2 * hp[shift + 1] + 3 * hp[shift + 2] +        
                         3 * hp[shift + 3] + 2 * hp[shift + 4] + hp[shift + 5]) / 12;
   //This is the DFT
   double cs, ss;
   for (int i = minperiod; i <= maxperiod; i++) {
      cs = ss = 0.0;
      for( int n = 0; n < maxperiod - 1; n++) {
        cs = cs + cleanedData[shift + n] * MathCos(2 * M_PI * n / i);
        ss = ss + cleanedData[shift + n] * MathSin(2 * M_PI * n / i);
      }
      Pwr[i] = MathPow(cs, 2) + MathPow(ss, 2);
   }
   //Find Maximum Power Level for Normalization
   double MaxPwr = Pwr[8];
   for (int i = minperiod; i <= maxperiod; i++) {
      if (Pwr[i] > MaxPwr)  MaxPwr = Pwr[i];
   }   
   
   //Normalize Power Levels and Convert to Decibels
   ArrayInitialize(DB, 0);  
   double Num = 0;
   double Denom = 0;   
   for (int i = minperiod; i <= maxperiod; i++) {
      if (MaxPwr > 0 && Pwr[i] > 0)
         DB[i] = -medianPeriod * MathLog(0.01 / (1 - 0.99 * Pwr[i] / MaxPwr)) / MathLog(10);
      if (DB[i] > decibelPeriod) DB[i] = decibelPeriod;
      if (DB[i] <= 3) {
         Num = Num + i * (decibelPeriod - DB[i]);
         Denom = Denom + (decibelPeriod - DB[i]);
      }
   }    
   if (Denom != 0.0) DomCycle[shift] = Num / Denom;   
   
   string l;
   color c;
   double v;
   for(int n = minperiod; n <= maxperiod; n++) {
      if(DB[n] <= 10) {
         v = 1 - DB[n] / 10 ; 
      } else {
         v = 2 - DB[n] / 10 ; 
      }
      l = NAME + "-" + IntegerToString(shift) + "-" + IntegerToString(n);
      c = ARGB(0, red * v, green * v, blue * v);      
      CreateOneRay(sub_win, l, n, shift, STYLE_SOLID, c  );
   }//for(int n = minperiod; n <= maxperiod; n++)     
}  

void CreateOneRay(
                          int             SubWin,
                          string          ObjName,
                          double          price, 
                          int             shift,
                          ENUM_LINE_STYLE line_style,
                          color           line_color 
                  )
  {
   datetime d1 = iTime(NULL, 0, shift);
   datetime d2 = iTime(NULL, 0, shift + 1);
	if (!ObjectCreate(0, ObjName, OBJ_TREND, SubWin, d1, price, d2, price) ){ return;}
        ObjectSetInteger(0, ObjName,OBJPROP_RAY_LEFT,false); 
        ObjectSetInteger(0, ObjName,OBJPROP_RAY_RIGHT,false);
        ObjectSetInteger(0, ObjName,OBJPROP_SELECTABLE,false);
        ObjectSetInteger(0, ObjName,OBJPROP_SELECTED,false);
        ObjectSetInteger(0, ObjName,OBJPROP_BACK,true);
        ObjectSetInteger(0, ObjName,OBJPROP_STYLE,line_style);
        ObjectSetInteger(0, ObjName,OBJPROP_WIDTH,iWidth);
        ObjectSetInteger(0, ObjName,OBJPROP_COLOR,line_color);
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
                const int begin,
                const double &price[])
  {
      if(rates_total < MINBAR) return 0;
      ArraySetAsSeries(price,true); 
      int limit = rates_total - prev_calculated;
      if (limit == 0)        { 
      } else if (limit == 1) {
         GetValue(price, 1);
         return(rates_total);  
      } else if (limit > 1)  {
         ArrayInitialize(DomCycle,   EMPTY_VALUE);
         ArrayInitialize(cleanedData,    0);
         ArrayInitialize(hp, 0);
         limit = rates_total - MINBAR;
         sub_win    = ChartWindowFind();
         for(int i = MathMin(limit,iMaxBar); i >= 1 && !IsStopped(); i--){
            GetValue(price, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
//      GetValue(price, 0);          

   return(rates_total);
  }
//+------------------------------------------------------------------+
void InitArrays(){
   ArrayResize(Pwr, maxperiod + 1);
   ArraySetAsSeries(Pwr, true);
   ArrayInitialize(Pwr, 0);  
   ArrayResize(DB, maxperiod + 1);
   ArraySetAsSeries(DB, true);
   ArrayInitialize(DB, 0);       
}  