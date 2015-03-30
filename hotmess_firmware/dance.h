/////////////////////////////////////////////////////////////////////////
// Unchanged MaKey MaKey code for dancing LEDs, IE during startup.     //
/////////////////////////////////////////////////////////////////////////
  
void initializeInputs();
void initializeArduino();
void danceLeds();

// input status LED pin numbers
const int inputLED_a = 9;
const int inputLED_b = 10;
const int inputLED_c = 11;
const int outputK = 14;
byte ledCycleCounter = 0;

//////////////////////
// SETUP /////////////
//////////////////////
void setup() 
{
  initializeArduino();
  initializeInputs();
  danceLeds();
}

///////////////////////////
// DANCE LEDS
///////////////////////////
void danceLeds()
{
  int delayTime = 50;
  int delayTime2 = 100;

  // CIRCLE
  for(int i=0; i<4; i++)
  {
    // UP
    pinMode(inputLED_a, INPUT);
    digitalWrite(inputLED_a, HIGH);
    pinMode(inputLED_b, OUTPUT);
    digitalWrite(inputLED_b, HIGH);
    pinMode(inputLED_c, OUTPUT);
    digitalWrite(inputLED_c, LOW);
    delay(delayTime);

    //RIGHT
    pinMode(inputLED_a, INPUT);
    digitalWrite(inputLED_a, LOW);
    pinMode(inputLED_b, OUTPUT);
    digitalWrite(inputLED_b, LOW);
    pinMode(inputLED_c, OUTPUT);
    digitalWrite(inputLED_c, HIGH);
    delay(delayTime);


    // DOWN
    pinMode(inputLED_a, OUTPUT);
    digitalWrite(inputLED_a, HIGH);
    pinMode(inputLED_b, OUTPUT);
    digitalWrite(inputLED_b, LOW);
    pinMode(inputLED_c, INPUT);
    digitalWrite(inputLED_c, LOW);
    delay(delayTime);

    // LEFT
    pinMode(inputLED_a, OUTPUT);
    digitalWrite(inputLED_a, LOW);
    pinMode(inputLED_b, OUTPUT);    
    digitalWrite(inputLED_b, HIGH);
    pinMode(inputLED_c, INPUT);
    digitalWrite(inputLED_c, LOW);
    delay(delayTime);    
  }    

  // WIGGLE
  for(int i=0; i<4; i++)
  {
    // SPACE
    pinMode(inputLED_a, OUTPUT);
    digitalWrite(inputLED_a, HIGH);
    pinMode(inputLED_b, INPUT);
    digitalWrite(inputLED_b, LOW);
    pinMode(inputLED_c, OUTPUT);
    digitalWrite(inputLED_c, LOW);
    delay(delayTime2);    

    // CLICK
    pinMode(inputLED_a, OUTPUT);
    digitalWrite(inputLED_a, LOW);
    pinMode(inputLED_b, INPUT);
    digitalWrite(inputLED_b, LOW);
    pinMode(inputLED_c, OUTPUT);
    digitalWrite(inputLED_c, HIGH);
    delay(delayTime2);    
  }
}

