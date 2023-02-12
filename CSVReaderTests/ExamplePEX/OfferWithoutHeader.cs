using CSVReader.Attributes;

namespace ExampleRecursive.PEX
{
    [DelimitedSet(
        trimPattern: ".*(?=PIT)",
        delimiters: "\t")]
    public class OfferWithoutHeader
        : Offer
    {
    }
}