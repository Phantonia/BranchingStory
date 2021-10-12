namespace Phantonia.BranchingStory.Markup
{
    public enum ErrorCode
    {
        None = 0,
        XmlSyntaxError,
        RootElementNotStory,
        TextElementWithChildren,
        OptWithoutId,
        SwitchChildNotOpt,
        BooleanAttributeNotTrueOrFalse,
        PreviousWithoutSwitchAttribute,
        PreviousTargetsNonAccessibleOrNonExistingSwitch,
        PreviousOptionDoesNotExistOnTargetedSwitch,
        PreviousChildNotOpt,
    }
}
