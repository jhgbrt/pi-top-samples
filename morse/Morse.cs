using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;

static class Morse
{
    public static ImmutableDictionary<char, string> morse = new (char alpha, string morse)[]
    {
        ('A' , ".-"),
        ('B' , "-..."),
        ('C' , "-.-."),
        ('D' , "-.."),
        ('E' , "."),
        ('F' , "..-."),
        ('G' , "--."),
        ('H' , "...."),
        ('I' , ".."),
        ('J' , ".---"),
        ('K' , "-.-"),
        ('L' , ".-.."),
        ('M' , "--"),
        ('N' , "-."),
        ('O' , "---"),
        ('P' , ".--."),
        ('Q' , "--.-"),
        ('R' , ".-."),
        ('S' , "..."),
        ('T' , "-"),
        ('U' , "..-"),
        ('V' , "...-"),
        ('W' , ".--"),
        ('X' , "-..-"),
        ('Y' , "-.--"),
        ('Z' , "--.."),
        ('0' , "-----"),
        ('1' , ".----"),
        ('2' , "..---"),
        ('3' , "...--"),
        ('4' , "....-"),
        ('5' , "....."),
        ('6' , "-...."),
        ('7' , "--..."),
        ('8' , "---.."),
        ('9' , "----."),
        (' ' , "/"),
    }.ToImmutableDictionary(x => x.alpha, x => x.morse);
    public static IEnumerable<string> ToMorse(this string alphanumeric) => 
        from c in alphanumeric.Normalize(NormalizationForm.FormD)
        let category = CharUnicodeInfo.GetUnicodeCategory(c)
        where category is UnicodeCategory.UppercaseLetter
        or UnicodeCategory.LowercaseLetter
        or UnicodeCategory.DecimalDigitNumber
        or UnicodeCategory.SpaceSeparator
        select morse[char.ToUpper(c)];
}
