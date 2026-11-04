using Domain.Handwriting;

namespace Infrastructure.Persistence.Configurations;

internal static class HandwritingJsonConstraints
{
    // Strict paths plus silent=true/IS TRUE reject missing fields, wrong types and JSON null.
    public static string Drawing(string column, bool reference) => $$"""
        jsonb_path_match({{column}}, 'strict
            $.type() == "array" && $.size() >= {{(reference ? 1 : 0)}} && $.size() <= {{InkDrawing.MaxStrokes}}
            && !exists($[*] ? (@.type() != "array" || @.size() < {{(reference ? 2 : 1)}} || @.size() > {{InkDrawing.MaxPointsPerStroke}}))
            && !exists($[*][*] ? (@.type() != "object" || ((exists(@.x)) is unknown) || ((exists(@.y)) is unknown)
                || @.x.type() != "number" || @.y.type() != "number"
                || @.x < 0 || @.x > 1 || @.y < 0 || @.y > 1))', '{}', true) IS TRUE
        AND jsonb_array_length(jsonb_path_query_array({{column}}, 'strict $[*][*]', '{}', true)) <= {{InkDrawing.MaxTotalPoints}}
        """;

    public static string Feedback => """
        jsonb_path_match(feedback, 'strict $.type() == "object"
            && $.passed.type() == "boolean" && $.matchScore.type() == "number"
            && $.matchScore >= 0 && $.matchScore <= 100 && $.matchScore % 1 == 0
            && $.expectedStrokeCount.type() == "number" && $.expectedStrokeCount >= 1 && $.expectedStrokeCount <= 12
            && $.expectedStrokeCount % 1 == 0
            && $.actualStrokeCount.type() == "number" && $.actualStrokeCount >= 0 && $.actualStrokeCount <= 12
            && $.actualStrokeCount % 1 == 0
            && $.issues.type() == "array" && $.issues.size() <= 8
            && $.strokes.type() == "array" && $.strokes.size() <= 12
            && !exists($.strokes[*] ? (@.type() != "object"
                || ((exists(@.index)) is unknown) || ((exists(@.passed)) is unknown) || ((exists(@.matchScore)) is unknown)
                || ((exists(@.distance)) is unknown) || ((exists(@.lengthRatio)) is unknown) || ((exists(@.issues)) is unknown)
                || @.index.type() != "number"
                || @.index < 0 || @.index >= 12 || @.index % 1 != 0
                || @.passed.type() != "boolean" || @.matchScore.type() != "number"
                || @.matchScore < 0 || @.matchScore > 100 || @.matchScore % 1 != 0
                || @.distance.type() != "number" || @.distance < 0
                || @.lengthRatio.type() != "number" || @.lengthRatio < 0
                || @.issues.type() != "array" || @.issues.size() > 8
                || (@.passed == true && @.issues.size() != 0)
                || (@.passed == false && @.issues.size() == 0)))', '{}', true) IS TRUE
        AND (feedback->>'actualStrokeCount')::integer = jsonb_array_length(drawing)
        AND (feedback->>'passed')::boolean = (jsonb_array_length(feedback->'issues') = 0)
        AND NOT jsonb_path_exists(feedback, 'strict $.issues[*] ? (@ != "MissingDrawing" && @ != "WrongStrokeCount"
            && @ != "DrawingTooSmall" && @ != "WrongStrokeOrder" && @ != "WrongDirection"
            && @ != "StrokeTooShort" && @ != "StrokeTooLong" && @ != "WrongShape")', '{}', true)
        AND NOT jsonb_path_exists(feedback, 'strict $.strokes[*].issues[*] ? (@ != "WrongStrokeOrder"
            && @ != "WrongDirection" && @ != "StrokeTooShort" && @ != "StrokeTooLong" && @ != "WrongShape")', '{}', true)
        AND ((jsonb_array_length(feedback->'strokes') = 0 AND (feedback->>'matchScore')::integer = 0
            AND jsonb_array_length(feedback->'issues') = 1 AND NOT (feedback->>'passed')::boolean
            AND ((feedback->'issues'->>0 = 'MissingDrawing' AND (feedback->>'actualStrokeCount')::integer = 0)
                OR (feedback->'issues'->>0 = 'WrongStrokeCount' AND (feedback->>'actualStrokeCount')::integer > 0
                    AND feedback->>'actualStrokeCount' <> feedback->>'expectedStrokeCount')
                OR (feedback->'issues'->>0 = 'DrawingTooSmall'
                    AND feedback->>'actualStrokeCount' = feedback->>'expectedStrokeCount')))
            OR (jsonb_array_length(feedback->'strokes') = (feedback->>'expectedStrokeCount')::integer
                AND feedback->>'actualStrokeCount' = feedback->>'expectedStrokeCount'
                AND (feedback->>'passed')::boolean = NOT jsonb_path_exists(feedback, 'strict $.strokes[*] ? (@.passed == false)')
                AND feedback->'issues' @> jsonb_path_query_array(feedback, 'strict $.strokes[*].issues[*]')
                AND jsonb_path_query_array(feedback, 'strict $.strokes[*].issues[*]') @> (feedback->'issues')))
        """ + "\nAND " + string.Join(" AND ", Enumerable.Range(0, InkDrawing.MaxStrokes).Select(index =>
            $"(jsonb_array_length(feedback->'strokes') <= {index} OR (feedback->'strokes'->{index}->>'index')::integer = {index})"));
}
