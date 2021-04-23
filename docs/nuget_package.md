# Tzolkin

A Maya  Tzolk’in date converter and calculator library.

The documentation is at [Read the Docs](https://tzolkin.readthedocs.io/en/latest/), the source code at [GitHub](https://github.com/Release-Candidate/Tzolkin).

## Install Package & Basic Usage

Download an install the `Tzolkin` Nuget packge using your IDE (Visual Studio or JetBrains Rider) or one of the command-line possibilities: [Tzolkin Package at NuGet](https://www.nuget.org/packages/Tzolkin/)

In an interactive F# FSI session, you can use the command `#r "nuget: NUGET_PACKAGE"` to download and use the nuget package `NUGET_PACKAGE`.


To use Tzolkin in this interactive session:


```F#
#r "nuget: Tzolkin"
```



    Installed package Tzolkin version 0.9.16


Everything is contained in the namespace `RC.Maya`, so let's open that


```F#
open RC.Maya
```

To check if everything's working, we call `TzolkinDate.today` to get todays (today is the 23th of April, 2021) Tzolk’in date - 7 Chikchan.

```F#
TzolkinDate.today
```

<table><thead><tr><th>Number</th><th>Glyph</th></tr></thead><tbody><tr><td><div class="dni-plaintext">7</div></td><td><div class="dni-plaintext">Chikchan</div></td></tr></tbody></table>

## Create Tzolk’in Dates

A Tzolk’in date consists of a Tzolk’in day number and a Tzolk’in day glyph. A day number is a int between 1 and 13 (including 1 and 13). A day glyph is one of the 20 names "Imix", "Ikʼ", "Akʼbʼal", "Kʼan", "Chikchan", "Kimi", "Manikʼ", "Lamat", "Muluk", "Ok", "Chuwen", "Ebʼ", "Bʼen", "Ix", "Men", "Kʼibʼ", "Kabʼan", "Etzʼnabʼ", "Kawak" and "Ajaw".

A Tzolk’in day number is represented by the type `TzolkinNumber.T` with a constructor `TzolkinNumber.T.TzolkinNumber`.

```F#
let tzolkinN = TzolkinNumber.T.TzolkinNumber 5
tzolkinN
```

<table><thead><tr><th>Item</th></tr></thead><tbody><tr><td><div class="dni-plaintext">5</div></td></tr></tbody></table>

But as only certain ints are allowed Tzolk’in day numbers, we have the constructor `TzolkinNumber.create`, that returns a `TzolkinNumber option`: `None` if the given int is non-positive (0 or negative) and values modulo 13 else, but with `13` instead of `0` if `n = 0 (mod13)`.

```F#
TzolkinNumber.create 3
```


<table><thead><tr><th>Value</th></tr></thead><tbody><tr><td><div class="dni-plaintext">3</div></td></tr></tbody></table>


```F#
TzolkinNumber.create 56
```




<table><thead><tr><th>Value</th></tr></thead><tbody><tr><td><div class="dni-plaintext">4</div></td></tr></tbody></table>




```F#
TzolkinNumber.create -8
```







```F#
TzolkinNumber.create 26
```




<table><thead><tr><th>Value</th></tr></thead><tbody><tr><td><div class="dni-plaintext">13</div></td></tr></tbody></table>



Same for Tzolk’in day glyphs `TzolkinGlyph`:


```F#
let tzolkinG = TzolkinGlyph.T.TzolkinGlyph 5
tzolkinG.ToString ()
```




    Chikchan




```F#
TzolkinGlyph.create 14
```




<table><thead><tr><th>Value</th></tr></thead><tbody><tr><td><div class="dni-plaintext">Ix</div></td></tr></tbody></table>




```F#
TzolkinGlyph.create -7
```







```F#
TzolkinGlyph.create 232
```




<table><thead><tr><th>Value</th></tr></thead><tbody><tr><td><div class="dni-plaintext">Ebʼ</div></td></tr></tbody></table>



We can also construct a `TzolkinGlyph` from a string using either `TzolkinGlyph.fromString`:


```F#
TzolkinGlyph.fromString "Kabʼan"
```




<table><thead><tr><th>Value</th></tr></thead><tbody><tr><td><div class="dni-plaintext">Kabʼan</div></td></tr></tbody></table>




```F#
TzolkinGlyph.fromString "Kab'an"
```






or `TzolkinGlyph.parseString` to also allow non-exact strings.


```F#
TzolkinGlyph.parseString "Kabʼan"
```




<table><thead><tr><th>Value</th></tr></thead><tbody><tr><td><div class="dni-plaintext">Kabʼan</div></td></tr></tbody></table>




```F#
TzolkinGlyph.parseString "Kab'an"
```




<table><thead><tr><th>Value</th></tr></thead><tbody><tr><td><div class="dni-plaintext">Kabʼan</div></td></tr></tbody></table>




```F#
TzolkinGlyph.parseString "K-a'b++AN"
```




<table><thead><tr><th>Value</th></tr></thead><tbody><tr><td><div class="dni-plaintext">Kabʼan</div></td></tr></tbody></table>



`TzolkinGlyph.parseString` ignores most punctuation characters, and is suitable to parse user input.

The list `TzolkinGlyph.glyphNames` contains all Tzolk’in day glyph names as string:


```F#
TzolkinGlyph.glyphNames
```




<table><thead><tr><th><i>index</i></th><th>value</th></tr></thead><tbody><tr><td>0</td><td><div class="dni-plaintext">Imix</div></td></tr><tr><td>1</td><td><div class="dni-plaintext">Ikʼ</div></td></tr><tr><td>2</td><td><div class="dni-plaintext">Akʼbʼal</div></td></tr><tr><td>3</td><td><div class="dni-plaintext">Kʼan</div></td></tr><tr><td>4</td><td><div class="dni-plaintext">Chikchan</div></td></tr><tr><td>5</td><td><div class="dni-plaintext">Kimi</div></td></tr><tr><td>6</td><td><div class="dni-plaintext">Manikʼ</div></td></tr><tr><td>7</td><td><div class="dni-plaintext">Lamat</div></td></tr><tr><td>8</td><td><div class="dni-plaintext">Muluk</div></td></tr><tr><td>9</td><td><div class="dni-plaintext">Ok</div></td></tr><tr><td>10</td><td><div class="dni-plaintext">Chuwen</div></td></tr><tr><td>11</td><td><div class="dni-plaintext">Ebʼ</div></td></tr><tr><td>12</td><td><div class="dni-plaintext">Bʼen</div></td></tr><tr><td>13</td><td><div class="dni-plaintext">Ix</div></td></tr><tr><td>14</td><td><div class="dni-plaintext">Men</div></td></tr><tr><td>15</td><td><div class="dni-plaintext">Kʼibʼ</div></td></tr><tr><td>16</td><td><div class="dni-plaintext">Kabʼan</div></td></tr><tr><td>17</td><td><div class="dni-plaintext">Etzʼnabʼ</div></td></tr><tr><td>18</td><td><div class="dni-plaintext">Kawak</div></td></tr><tr><td>19</td><td><div class="dni-plaintext">Ajaw</div></td></tr></tbody></table>



A Tzolk’in day `TzolkinDate` is a record with two fields, `Number` and `Glyph`:


```F#
let tzolkin = { TzolkinDate.Number = TzolkinNumber.T.TzolkinNumber 13; TzolkinDate.Glyph = TzolkinGlyph.T.TzolkinGlyph 17 }
tzolkin
```




<table><thead><tr><th>Number</th><th>Glyph</th></tr></thead><tbody><tr><td><div class="dni-plaintext">13</div></td><td><div class="dni-plaintext">Kabʼan</div></td></tr></tbody></table>



It can be created with a `TzolkinNumber` and a `TzolkinGlyph`:


```F#
let tzolkinN = TzolkinNumber.T.TzolkinNumber 13
let tzolkinG = TzolkinGlyph.T.TzolkinGlyph 17

TzolkinDate.create tzolkinN tzolkinG
```




<table><thead><tr><th>Number</th><th>Glyph</th></tr></thead><tbody><tr><td><div class="dni-plaintext">13</div></td><td><div class="dni-plaintext">Kabʼan</div></td></tr></tbody></table>



But this does not check the input, so invalid dates can be a result.


```F#
let tzolkinN = TzolkinNumber.T.TzolkinNumber 65
let tzolkinG = TzolkinGlyph.T.TzolkinGlyph 6

TzolkinDate.create tzolkinN tzolkinG
```




<table><thead><tr><th>Number</th><th>Glyph</th></tr></thead><tbody><tr><td><div class="dni-plaintext">65</div></td><td><div class="dni-plaintext">Kimi</div></td></tr></tbody></table>



The constructor `TzolkinDate.fromInts` checks the input and returns an `TzolkinDate option`:


```F#
TzolkinDate.fromInts 13 17
```




<table><thead><tr><th>Value</th></tr></thead><tbody><tr><td><div class="dni-plaintext">13 Kabʼan</div></td></tr></tbody></table>




```F#
TzolkinDate.fromInts -3 14
```







```F#
TzolkinDate.fromInts 13 -6
```






There exists two constructor to create a `TzolkinDate` from strings.

`TzolkinDate.fromStrings` creates a `TzolkinDate option` from 2 strings:


```F#
TzolkinDate.fromStrings "13" "Kabʼan"
```




<table><thead><tr><th>Value</th></tr></thead><tbody><tr><td><div class="dni-plaintext">13 Kabʼan</div></td></tr></tbody></table>




```F#
TzolkinDate.fromStrings "-3" "Kabʼan"
```







```F#
TzolkinDate.fromStrings "13" "FOO"
```






`TzolkinDate.fromString` creates a `TzolkinDate` from a combined date. The day number and day glyph can be separated by whitespace, by a punctuation character or not at all:


```F#
TzolkinDate.fromString "13 Kabʼan"
```




<table><thead><tr><th>Value</th></tr></thead><tbody><tr><td><div class="dni-plaintext">13 Kabʼan</div></td></tr></tbody></table>




```F#
TzolkinDate.fromString "13-Kabʼan"
```




<table><thead><tr><th>Value</th></tr></thead><tbody><tr><td><div class="dni-plaintext">13 Kabʼan</div></td></tr></tbody></table>




```F#
TzolkinDate.fromString "13Kabʼan"
```




<table><thead><tr><th>Value</th></tr></thead><tbody><tr><td><div class="dni-plaintext">13 Kabʼan</div></td></tr></tbody></table>



Both `TzolkinDate.fromString` and `TzolkinDate.fromStrings` parse the glyph name string and ignore almost all punctuation characters in it. So these functions are suitable to parse user input.


```F#
TzolkinDate.fromString "13 kA*b`aN"
```




<table><thead><tr><th>Value</th></tr></thead><tbody><tr><td><div class="dni-plaintext">13 Kabʼan</div></td></tr></tbody></table>




```F#
TzolkinDate.fromStrings "13" "k*a-b´`^aN#"
```




<table><thead><tr><th>Value</th></tr></thead><tbody><tr><td><div class="dni-plaintext">13 Kabʼan</div></td></tr></tbody></table>



## Conversion to Integers and Strings

`TzolkinGlyph` can be converted to int and string, using `int` and either the function `toString` or the member function `ToString`:


```F#
let tzolkinG = TzolkinGlyph.T.TzolkinGlyph 17

TzolkinGlyph.toString tzolkinG
```




    Kabʼan




```F#
tzolkinG.ToString ()
```




    Kabʼan




```F#
int tzolkinG
```




<div class="dni-plaintext">17</div>



A `TzolkinNumber` can be converted to an int using `int`:


```F#
let tzolkinN = TzolkinNumber.T.TzolkinNumber 13

int tzolkinN
```




<div class="dni-plaintext">13</div>



A `TzolkinDate` can be converted to a string using it's member function `ToString`:


```F#
let tzolkin = TzolkinDate.create (TzolkinNumber.T.TzolkinNumber 13) (TzolkinGlyph.T.TzolkinGlyph 17)

tzolkin.ToString ()
```




    13 Kabʼan



## Convert Gregorian Dates to Tzolk’in Dates

We can get the Tzolk’in date of the current day using `TzolkinDate.today` (today is the 23th of April, 2021, 7 Chikchan)


```F#
TzolkinDate.today
```




<table><thead><tr><th>Number</th><th>Glyph</th></tr></thead><tbody><tr><td><div class="dni-plaintext">7</div></td><td><div class="dni-plaintext">Chikchan</div></td></tr></tbody></table>



To get the Tzolk’in date of a given Gregorian date, there are the functions `TzolkinDate.fromDate` - to convert from a `DateTime`. We use the 25th of May, 2021.


```F#
TzolkinDate.fromDate (DateTime (2021, 05, 25))
```




<table><thead><tr><th>Number</th><th>Glyph</th></tr></thead><tbody><tr><td><div class="dni-plaintext">13</div></td><td><div class="dni-plaintext">Kabʼan</div></td></tr></tbody></table>



`TzolkinDate.fromDateString` to convert from a date string given and a date format string


```F#
TzolkinDate.fromDateString "25.05.2021" "dd.MM.yyyy"
```




<table><thead><tr><th>Number</th><th>Glyph</th></tr></thead><tbody><tr><td><div class="dni-plaintext">13</div></td><td><div class="dni-plaintext">Kabʼan</div></td></tr></tbody></table>



`TzolkinDate.fromISOString` to convert from a ISO date string, the format is "yyyy-MM-dd"


```F#
TzolkinDate.fromISOString "2021-05-25"
```




<table><thead><tr><th>Number</th><th>Glyph</th></tr></thead><tbody><tr><td><div class="dni-plaintext">13</div></td><td><div class="dni-plaintext">Kabʼan</div></td></tr></tbody></table>



We can also get the Tzolk’in day number of a Gregorian date by using `TzolkinNumber.fromDate`:


```F#
TzolkinNumber.fromDate (DateTime (2021, 05, 25))
```




<table><thead><tr><th>Item</th></tr></thead><tbody><tr><td><div class="dni-plaintext">13</div></td></tr></tbody></table>



The Tzolk’in day glyph we can get using `TzolkinGlyph.fromDate`.


```F#
let tzolkin = TzolkinGlyph.fromDate (DateTime (2021, 05, 25))
tzolkin
```




<table><thead><tr><th>Item</th></tr></thead><tbody><tr><td><div class="dni-plaintext">17</div></td></tr></tbody></table>



A `TzolkinGlyph` is an int between 1 and 20 (including 1 and 20), so we need to use `toString` or the member function `ToString` to get the name.


```F#
TzolkinGlyph.toString tzolkin
```




    Kabʼan




```F#
tzolkin.ToString ()
```




    Kabʼan



## Find Tzolk’in Dates

To find a Tzolk’in date before or after a given Gregorian Date, there exist the functions `getNext` and `getLast`.

To get the next day after the given Gregorian date with the Tzolk’in date we search for, we use `getNext`. To get the next Gregorian date with a Tzolk’in date of 13 Kabʼan after the 23th of April, 2021. We should get the 25th of May, 2021 as a result.


```F#
let tzolkin = TzolkinDate.create (TzolkinNumber.T.TzolkinNumber 13) (TzolkinGlyph.T.TzolkinGlyph 17)

TzolkinDate.getNext tzolkin (DateTime (2021, 04, 23))
```




<span>2021-05-25 00:00:00Z</span>



`getLast` returns the Gregorian date before the start date.


```F#
TzolkinDate.getLast tzolkin (DateTime (2021, 05, 30))
```




<span>2021-05-25 00:00:00Z</span>



TzolkinGlpyh and TzolkinNumber have both `getNext` and `getLast` too:


```F#
let tzolkinG = TzolkinGlyph.T.TzolkinGlyph 17

TzolkinGlyph.getNext tzolkinG (DateTime (2021, 05, 16))
```




<span>2021-05-25 00:00:00Z</span>




```F#
let tzolkinN = TzolkinNumber.T.TzolkinNumber 13

TzolkinNumber.getLast tzolkinN (DateTime (2021, 05, 30))
```




<span>2021-05-25 00:00:00Z</span>



We can get lists of Gregorian dates with the same Tzolk’in date after or before a start date by using `getNextList` and `getLastList`.

Get the list of 10 dates with a Tzolk’in date of 13 Kabʼan after the 23th of April, 2021:


```F#
let tzolkin =  TzolkinDate.create (TzolkinNumber.T.TzolkinNumber 13) (TzolkinGlyph.T.TzolkinGlyph 17)

TzolkinDate.getNextList 10 tzolkin (DateTime (2021, 04, 23))
```




<table><thead><tr><th><i>index</i></th><th>value</th></tr></thead><tbody><tr><td>0</td><td><span>2021-05-25 00:00:00Z</span></td></tr><tr><td>1</td><td><span>2022-02-09 00:00:00Z</span></td></tr><tr><td>2</td><td><span>2022-10-27 00:00:00Z</span></td></tr><tr><td>3</td><td><span>2023-07-14 00:00:00Z</span></td></tr><tr><td>4</td><td><span>2024-03-30 00:00:00Z</span></td></tr><tr><td>5</td><td><span>2024-12-15 00:00:00Z</span></td></tr><tr><td>6</td><td><span>2025-09-01 00:00:00Z</span></td></tr><tr><td>7</td><td><span>2026-05-19 00:00:00Z</span></td></tr><tr><td>8</td><td><span>2027-02-03 00:00:00Z</span></td></tr><tr><td>9</td><td><span>2027-10-21 00:00:00Z</span></td></tr></tbody></table>



`getLastList` includes the start date, if the start date has a Tzolk’in date that we are searching for, `getNextList` does not incllude the start that in that case.


```F#
TzolkinDate.getLastList 5 tzolkin (DateTime (2021, 05, 25))
```




<table><thead><tr><th><i>index</i></th><th>value</th></tr></thead><tbody><tr><td>0</td><td><span>2021-05-25 00:00:00Z</span></td></tr><tr><td>1</td><td><span>2020-09-07 00:00:00Z</span></td></tr><tr><td>2</td><td><span>2019-12-22 00:00:00Z</span></td></tr><tr><td>3</td><td><span>2019-04-06 00:00:00Z</span></td></tr><tr><td>4</td><td><span>2018-07-20 00:00:00Z</span></td></tr><tr><td>5</td><td><span>2017-11-02 00:00:00Z</span></td></tr></tbody></table>



`TzolkinNumber` and `TzolkinGlyph` have these functions too:


```F#
let tzolkinN = TzolkinNumber.T.TzolkinNumber 13

TzolkinNumber.getNextList 5 tzolkinN (DateTime (2021, 05, 16))
```




<table><thead><tr><th><i>index</i></th><th>value</th></tr></thead><tbody><tr><td>0</td><td><span>2021-05-25 00:00:00Z</span></td></tr><tr><td>1</td><td><span>2021-06-07 00:00:00Z</span></td></tr><tr><td>2</td><td><span>2021-06-20 00:00:00Z</span></td></tr><tr><td>3</td><td><span>2021-07-03 00:00:00Z</span></td></tr><tr><td>4</td><td><span>2021-07-16 00:00:00Z</span></td></tr></tbody></table>




```F#
let tzolkinG = TzolkinGlyph.T.TzolkinGlyph 17

TzolkinGlyph.getLastList 10 tzolkinG (DateTime (2021, 05, 30))
```




<table><thead><tr><th><i>index</i></th><th>value</th></tr></thead><tbody><tr><td>0</td><td><span>2021-05-25 00:00:00Z</span></td></tr><tr><td>1</td><td><span>2021-05-05 00:00:00Z</span></td></tr><tr><td>2</td><td><span>2021-04-15 00:00:00Z</span></td></tr><tr><td>3</td><td><span>2021-03-26 00:00:00Z</span></td></tr><tr><td>4</td><td><span>2021-03-06 00:00:00Z</span></td></tr><tr><td>5</td><td><span>2021-02-14 00:00:00Z</span></td></tr><tr><td>6</td><td><span>2021-01-25 00:00:00Z</span></td></tr><tr><td>7</td><td><span>2021-01-05 00:00:00Z</span></td></tr><tr><td>8</td><td><span>2020-12-16 00:00:00Z</span></td></tr><tr><td>9</td><td><span>2020-11-26 00:00:00Z</span></td></tr></tbody></table>



## Calculations with Tzolk’in Dates

We can add ints - meaning days - and `TimeSpan`s to the 3 Tzolk’in types:


```F#
let tzolkinN = TzolkinNumber.T.TzolkinNumber 13

tzolkinN + -6
```




<table><thead><tr><th>Item</th></tr></thead><tbody><tr><td><div class="dni-plaintext">7</div></td></tr></tbody></table>




```F#
let tzolkinG = TzolkinGlyph.T.TzolkinGlyph 17

8 + tzolkinG
```




<table><thead><tr><th>Item</th></tr></thead><tbody><tr><td><div class="dni-plaintext">5</div></td></tr></tbody></table>




```F#
let tzolkin =  TzolkinDate.create (TzolkinNumber.T.TzolkinNumber 13) (TzolkinGlyph.T.TzolkinGlyph 17)

tzolkin + TimeSpan.FromDays -7.0
```




<table><thead><tr><th>Number</th><th>Glyph</th></tr></thead><tbody><tr><td><div class="dni-plaintext">6</div></td><td><div class="dni-plaintext">Ok</div></td></tr></tbody></table>




```F#
TimeSpan.FromDays 186. + tzolkinN
```




<table><thead><tr><th>Item</th></tr></thead><tbody><tr><td><div class="dni-plaintext">4</div></td></tr></tbody></table>



And we can calculate the difference in days between two TzolkinDates, TzolkinNumbers or TzolkinGlyphs by subtracting them.


```F#
let tzolkinN1 = TzolkinNumber.T.TzolkinNumber 13
let tzolkinN2 = TzolkinNumber.T.TzolkinNumber 7

tzolkinN2 - tzolkinN1
```




<div class="dni-plaintext">-6</div>




```F#
let tzolkinG1 = TzolkinGlyph.T.TzolkinGlyph 6
let tzolkinG2 = TzolkinGlyph.T.TzolkinGlyph 17

tzolkinG2 - tzolkinG1
```




<div class="dni-plaintext">11</div>



The difference between 2 `TzolkinDate`s is always non-negative and between 0 and 260 (including 0 and 260). So, given `tz1 - tz2`, the result is `int tz1 - int tz2` if `tz1 > tz2` and is `260 - int tz2 + int tz1` (`260 - (int tz2 - int tz1)`) else.


```F#
let tzolkin1 =  TzolkinDate.create (TzolkinNumber.T.TzolkinNumber 13) (TzolkinGlyph.T.TzolkinGlyph 17)
let tzolkin2 =  TzolkinDate.create (TzolkinNumber.T.TzolkinNumber 6) (TzolkinGlyph.T.TzolkinGlyph 2)

tzolkin2 - tzolkin1
```




<div class="dni-plaintext">45</div>




```F#
tzolkin1 - tzolkin2
```




<div class="dni-plaintext">215</div>



## Tzolk’in Year, Glyph descriptions and Unicode Glyphs

We get a F# map of all 260 days to the Tzolk’in days in the Tzolk’in year using `TzolkinDate.yearMap`, the same map as strings we get by using `TzolkinDate.yearStringMap`.


```F#
TzolkinDate.yearStringMap ()
```




<table><thead><tr><th><i>key</i></th><th>value</th></tr></thead><tbody><tr><td><div class="dni-plaintext">1</div></td><td><div class="dni-plaintext">1 Imix</div></td></tr><tr><td><div class="dni-plaintext">2</div></td><td><div class="dni-plaintext">2 Ikʼ</div></td></tr><tr><td><div class="dni-plaintext">3</div></td><td><div class="dni-plaintext">3 Akʼbʼal</div></td></tr><tr><td><div class="dni-plaintext">4</div></td><td><div class="dni-plaintext">4 Kʼan</div></td></tr><tr><td><div class="dni-plaintext">5</div></td><td><div class="dni-plaintext">5 Chikchan</div></td></tr><tr><td><div class="dni-plaintext">6</div></td><td><div class="dni-plaintext">6 Kimi</div></td></tr><tr><td><div class="dni-plaintext">7</div></td><td><div class="dni-plaintext">7 Manikʼ</div></td></tr><tr><td><div class="dni-plaintext">8</div></td><td><div class="dni-plaintext">8 Lamat</div></td></tr><tr><td><div class="dni-plaintext">9</div></td><td><div class="dni-plaintext">9 Muluk</div></td></tr><tr><td><div class="dni-plaintext">10</div></td><td><div class="dni-plaintext">10 Ok</div></td></tr><tr><td><div class="dni-plaintext">11</div></td><td><div class="dni-plaintext">11 Chuwen</div></td></tr><tr><td><div class="dni-plaintext">12</div></td><td><div class="dni-plaintext">12 Ebʼ</div></td></tr><tr><td><div class="dni-plaintext">13</div></td><td><div class="dni-plaintext">13 Bʼen</div></td></tr><tr><td><div class="dni-plaintext">14</div></td><td><div class="dni-plaintext">1 Ix</div></td></tr><tr><td><div class="dni-plaintext">15</div></td><td><div class="dni-plaintext">2 Men</div></td></tr><tr><td><div class="dni-plaintext">16</div></td><td><div class="dni-plaintext">3 Kʼibʼ</div></td></tr><tr><td><div class="dni-plaintext">17</div></td><td><div class="dni-plaintext">4 Kabʼan</div></td></tr><tr><td><div class="dni-plaintext">18</div></td><td><div class="dni-plaintext">5 Etzʼnabʼ</div></td></tr><tr><td><div class="dni-plaintext">19</div></td><td><div class="dni-plaintext">6 Kawak</div></td></tr><tr><td><div class="dni-plaintext">20</div></td><td><div class="dni-plaintext">7 Ajaw</div></td></tr><tr><td colspan="2">(240 more)</td></tr></tbody></table>



`TzolkinDate.dayInYear` returns the number of the Tzolk’in date in the Tzolk’in year, an inte between 1 and 260 (including 1 and 260). 13 Kabʼan is the 117th day of the Tzolk’in year:


```F#
let tzolkin =  TzolkinDate.create (TzolkinNumber.T.TzolkinNumber 13) (TzolkinGlyph.T.TzolkinGlyph 17)

TzolkinDate.dayInYear tzolkin
```




<div class="dni-plaintext">117</div>



This is the same as the function `toInt`:


```F#
TzolkinDate.toInt tzolkin
```




<div class="dni-plaintext">117</div>



You can get the description contained in a `TzolkinGlyph.GlyphDescription` record of a Tzolk’in day glyph in spanish using the function `TzolkinGlyph.getDescription`:


```F#
let tzolkinG = TzolkinGlyph.T.TzolkinGlyph 17

TzolkinGlyph.getDescription tzolkinG
```




<table><thead><tr><th>Meaning</th><th>ElementOrAnimal</th><th>Direction</th><th>Color</th><th>God</th><th>Url</th></tr></thead><tbody><tr><td><div class="dni-plaintext">Tierra</div></td><td><div class="dni-plaintext">Tierra y los temblores</div></td><td><div class="dni-plaintext">sur</div></td><td><div class="dni-plaintext">amarillo</div></td><td><div class="dni-plaintext">diosa I o diosa de la sensualidad y el amor</div></td><td><div class="dni-plaintext">https://arqueologiamexicana.mx/dias-mayas</div></td></tr></tbody></table>



`TzolkinNumber` and `TzolkinGlyph` contain a function `toUnicode`, that returns the Unicode code point for the Tzolk’in day number or Tzolk’in day glyph. As of now, these code points are not yet part of the Unicode standard, but will be included in the future. I made a TTF font containing these numbers and glyphs, that you can use under the CC BY-SA 4.0 license at [GitHub](https://github.com/Release-Candidate/Tzolkin/blob/main/src/Tzolkin/Tzolkin.ttf).


```F#
let tzolkin =  TzolkinDate.create (TzolkinNumber.T.TzolkinNumber 13) (TzolkinGlyph.T.TzolkinGlyph 17)

TzolkinNumber.toUnicode tzolkin.Number
```




    𕎱




```F#
TzolkinGlyph.toUnicode tzolkin.Glyph
```




    𕐖



&copy; 2021 Roland Csaszar, licensed under the MIT license.
