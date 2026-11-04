using System.Text.Json.Serialization;

namespace HandwritingLab.Models;

public readonly record struct InkPoint([property: JsonRequired] double X, [property: JsonRequired] double Y);
