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

#property indicator_buffers 4
#property indicator_plots   1
//--- plot V1
#property indicator_label1  "V1"
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

//long cChart = CLR_NONE;
int red, green, blue;
//--- indicator buffers
double         v1[];
double smoothHp[], hp[], dc[];

double Q[];
double I[];
double Real[];
double Imag[];
double Ampl[];
double OldQ[];
double OldI[];
double OlderQ[];
double OlderI[];
double OldReal[];
double OldImag[];
double OlderReal[];
double OlderImag[];
double OldAmpl[];
double DB[];


static const int MINBAR = maxperiod + 1;
double a1, a2;
int sub_win;
string NAME = "SpectrumDFB";

#define ARGB(a,r,g,b)  ((uchar(a)<<24)|(uchar(r)<<16)|(uchar(g)<<8)|uchar(b))
#define GETRGBR(clr)   uchar((clr)>>16)
#define GETRGBG(clr)   uchar((clr)>>8)
#define GETRGBB(clr)   uchar(clr)

//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
int OnInit()
  {
//--- indicator buffers mapping
   SetIndexBuffer(0,v1,INDICATOR_DATA);
   SetIndexBuffer(1,smoothHp,INDICATOR_CALCULATIONS);
   SetIndexBuffer(2,dc,INDICATOR_CALCULATIONS);
   SetIndexBuffer(3,hp,INDICATOR_CALCULATIONS);
   
   ArraySetAsSeries(v1,true);
   ArraySetAsSeries(smoothHp,true); 
   ArraySetAsSeries(dc,true); 
   ArraySetAsSeries(hp,true); 
//---
   int i = 1;
   string n = NAME;
   while (ChartWindowFind(0, n) != -1) n = NAME + IntegerToString(i++);
   NAME = n;
   
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

   hp[shift] =  a2 * (price[shift] - price[shift + 1]) + a1 * hp[shift + 1];
   smoothHp[shift] = (hp[shift] + 2 * hp[shift + 1] + 3 * hp[shift + 2] +        
                  3 * hp[shift + 3] + 2 * hp[shift + 4] + hp[shift + 5]) / 12;
   double delta = -0.015 * shift + 0.5;
   if (delta < 0.15) delta =  0.15;
   
   double num = 0.0, denom = 0.0; 
   double maxAmpl = 0.0;
   double s1 = smoothHp[shift] - smoothHp[shift + 1];
   double beta, gamma, alpha;
   for (int n = minperiod; n <= maxperiod; n++) {
      beta = MathCos(2 * M_PI / n);
      gamma = 1 / MathCos(4 * M_PI * delta / n);
      alpha = gamma - MathSqrt(MathPow(gamma, 2) - 1);
      Q[n] = (n / 6.283185) * s1;
      I[n] = smoothHp[shift];
      Real[n] = 0.5 * (1 - alpha) * (I[n] - OlderI[n]) + beta * (1 + alpha) * OldReal[n] - alpha * OlderReal[n];
      Imag[n] = 0.5 * (1 - alpha) * (Q[n] - OlderQ[n]) + beta * (1 + alpha) * OldImag[n] - alpha * OlderImag[n];
      Ampl[n] = MathPow(Real[n], 2) + MathPow(Imag[n], 2);   
   }//for (int n = minperiod; n <= maxperiod; n++)  
   
   double   MaxAmpl = Ampl[medianPeriod];
   for (int n = minperiod; n <= maxperiod; n++) {
      OlderI[n] = OldI[n];
      OldI[n] = I[n];
      OlderQ[n] = OldQ[n];
      OldQ[n] = Q[n];
      OlderReal[n] = OldReal[n];
      OldReal[n] = Real[n];
      OlderImag[n] = OldImag[n];
      OldImag[n] = Imag[n];
      OldAmpl[n] = Ampl[n];
      if(Ampl[n] > MaxAmpl) MaxAmpl = Ampl[n];
   }    

   for(int n = minperiod; n <= maxperiod; n++) {
      if(MaxAmpl != 0 && (Ampl[n] / MaxAmpl) > 0)
         DB[n] = -medianPeriod * MathLog(.01 / (1 - 0.99 * Ampl[n] / MaxAmpl)) / MathLog(10);
      if(DB[n] > decibelPeriod) DB[n] = decibelPeriod;
      
      if(DB[n] <= 3) {
         num   = num   + n * (decibelPeriod - DB[n]);
         denom = denom +     (decibelPeriod - DB[n]);
      }
      
   }//for(int n = minperiod; n <= maxperiod; n++)   
   if(denom != 0) dc[shift] = num / denom; 
   double domCyc = MathMedian(dc, shift, medianPeriod);
   
   v1[shift] = domCyc;
   if (shift > iMaxBar) return;    
   
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
                          int             SubWin, // номер окна
                          string          ObjName,   // имя объекта
                          double          price,         // уровень цены
                          int             shift,
                          ENUM_LINE_STYLE line_style,    // стиль линии
                          color           line_color     // цвет линии
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
  ObjectsDeleteAll(0, NAME);
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
      if(rates_total < MINBAR) return 0;
      ArraySetAsSeries(price,true); 
      int limit = rates_total - prev_calculated;
      if (limit == 0)        {   // Пришел новый тик 
      } else if (limit == 1) {   // Образовался новый бар
         GetValue(price, 1);
         return(rates_total);
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(v1,   EMPTY_VALUE);
         ArrayInitialize(smoothHp,    0);
         ArrayInitialize(dc, 0);
         ArrayInitialize(hp, 0);
         limit = rates_total - MINBAR;
         sub_win    = ChartWindowFind();
         for(int i = MathMin(limit,iMaxBar); i >= 1 && !IsStopped(); i--){
            GetValue(price, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
      GetValue(price, 0);          

   return(rates_total);
  }
//+------------------------------------------------------------------+

void InitArrays(){
   ArrayResize(Q, maxperiod + 1);
   ArraySetAsSeries(Q, true); 
   ArrayInitialize(Q, 0);
   ArrayResize(I, maxperiod + 1);
   ArraySetAsSeries(I, true);
   ArrayInitialize(I, 0);    
   ArrayResize(Real, maxperiod + 1);
   ArraySetAsSeries(Real, true);
   ArrayInitialize(Real, 0);    
   ArrayResize(Imag, maxperiod + 1);
   ArraySetAsSeries(Imag, true);
   ArrayInitialize(Imag, 0);    
   ArrayResize(Ampl, maxperiod + 1);
   ArraySetAsSeries(Ampl, true);
   ArrayInitialize(Ampl, 0);    
   ArrayResize(OldQ, maxperiod + 1);
   ArraySetAsSeries(OldQ, true);
   ArrayInitialize(OldQ, 0);    
   ArrayResize(OldI, maxperiod + 1);
   ArraySetAsSeries(OldI, true);
   ArrayInitialize(OldI, 0);    
   ArrayResize(OlderQ, maxperiod + 1);
   ArraySetAsSeries(OlderQ, true);
   ArrayInitialize(OlderQ, 0);    
   ArrayResize(OlderI, maxperiod + 1);
   ArraySetAsSeries(OlderI, true);
   ArrayInitialize(OlderI, 0);    
   ArrayResize(OldReal, maxperiod + 1);
   ArraySetAsSeries(OldReal, true);
   ArrayInitialize(OldReal, 0);    
   ArrayResize(OldImag, maxperiod + 1);
   ArraySetAsSeries(OldImag, true);
   ArrayInitialize(OldImag, 0);    
   ArrayResize(OlderReal, maxperiod + 1);
   ArraySetAsSeries(OlderReal, true);
   ArrayInitialize(OlderReal, 0);    
   ArrayResize(OlderImag, maxperiod + 1);
   ArraySetAsSeries(OlderImag, true);
   ArrayInitialize(OlderImag, 0);    
   ArrayResize(OldAmpl, maxperiod + 1);
   ArraySetAsSeries(OldAmpl, true);
   ArrayInitialize(OldAmpl, 0);    
   ArrayResize(DB, maxperiod + 1);
   ArraySetAsSeries(DB, true);
   ArrayInitialize(DB, 0);    
}  