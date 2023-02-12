using CSVReader.Attributes;

namespace ExampleRecursive.PEX
{
    [DelimitedSet(
        headerPattern: "PIF|PEX",
        trimPattern: ".*(?=PIT)",
        delimiters: "\t")]
    public class OfferWithHeader
        : Offer
    {
    }
}