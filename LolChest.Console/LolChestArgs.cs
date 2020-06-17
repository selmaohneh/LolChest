using PowerArgs;

namespace LolChest.Console
{
    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    public class LolChestArgs
    {
        [ArgShortcut("y")]
        [ArgRequired(PromptIfMissing = true)]
        [ArgDescription("The year for which the matches will be polled")]
        public int Year { get; set; }

        [ArgShortcut("m")]
        [ArgDescription("The number of the month for which the matches will be polled")]
        [ArgRequired(PromptIfMissing = true)]
        [ArgRange(1, 12)]
        public int Month { get; set; }
    }
}