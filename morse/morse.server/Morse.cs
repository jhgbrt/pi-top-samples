using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;

static class MorseEx
{
    // see:
    //  https://www.itu.int/dms_pubrec/itu-r/rec/m/R-REC-M.1677-1-200910-I!!PDF-E.pdf
    //  https://morsecode.world/international/morse2.html
    static ImmutableDictionary<char, string> Alphabet = new (char alpha, string morse)[]
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
        ('.', ".-.-.-"),
        (',', "--..--"),
        (':', "---..."),
        ('?', "..--.."),
        ('\'', ".----."),
        ('-', "-....-"),
        ('/', "-..-."),
        ('(', "-.--."),
        (')', "-.--.-"),
        ('"', ".-..-."),
        (' ' , "/"),
        ('\t' , "/"),
        ('=', "-...-"),
        ('!', "-.-.--"),
        ('+', ".-.-."),
        ('@', ".--.-."),
        ('&', ".-...")
    }.ToImmutableDictionary(x => x.alpha, x => x.morse);

    public static string ToMorse(this char c) => Alphabet[char.ToUpper(c)];
    public static string ToMorse(this string alpha) => string.Join(" ", alpha.ToMorseImpl());
    static IEnumerable<string> ToMorseImpl(this string alphanumeric) =>
        from c in alphanumeric.Normalize(NormalizationForm.FormD)
        let u = char.ToUpper(c)
        where Alphabet.ContainsKey(u)
        select Alphabet[char.ToUpper(u)];
}
