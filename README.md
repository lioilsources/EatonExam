# EatonExam

# Assignment
## The problem to solve is following:
* You monitor devices, which are sending data to you.
* Each device have a unique name.
* Each device produces measurements.
 
## Challange is:
* Compute number of messages you got or read from the devices.

## Note
* The solution can be in any preferable language (C/C++, Java, Javascript, Python, C#, …).
* The scope is open, you must decide how the “devices” will work in your system.
* The solution should be posted on GitHub or a similar page for a review.
* Please add Makefile and documentation explaining us how to run your code.

# Řešení
Největší výzva spočívá v poznámce pod čarou a tou je "rozhodněte, jak se budou zařízení ve vašem systému chovat".
Rozhodl jsem se pro model vícevláknového systému, kde jak měřící zařízení, tak monitorovací zařízení běží 
ve vlastních vláknech. Rozhodl jsem se pro synchronizaci měřících zařízení s monitorem měření pomocí třídy CountdownEvent.
Řešení předpokládá, že měřící zařízení musí sama rozhodnout, kdy skončí. Bez tohoto rozhodnutí, není možné měření zobrazit.

# Nedostatky řešení
1. Api třídy DevicesEnvironment je nedokonalé, není možné měření nejprve vypsat a potom spustit měřící zařízení.
1. Nelíbí se mi nedostatečné zapouzdření synchronizačního objektu, kdy je nutné k němu přistupovat přímo z metody měřícího zařízení.
1. V reálném systému monitor pravděpodobně nebude čekat na to, až mu měřící zařízení řeknou, že je možné měření vypsat, toto bylo
použito pouze pro zjednodušení úlohy a zajištění, že jsou vypsány opravdu všechna provedená měření.

# Spuštění
Jedná se o projekt prostředí VisualStudio. Počet měřících zařízení je možné měnit v parametru konstruktoru třídy DevicesEnvironment. 
Parametry měřících zařízení jako je rychlost měření, počet měření a hodnota měření jsou vybrány náhodně.
