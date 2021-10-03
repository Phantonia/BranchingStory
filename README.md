# BranchingStory
BranchingStory is a framework for stories where decisions matter.

The way this works is by feeding scripts in an XML based language which specify the flow of the story into the library. You get an instance of the class `Phantonia.BranchingStory.Story`. This class is the focal point of the logic because it handles all of the complicated progress logic. All you need to do is call `Progress` or `ProgressWithOption` and consume the information.

An example for a simple script in Branching Story Markup Language (bsml):

```xml
<story>
	<text char="Queen">Kris, Get The Banana.</text>
    
    <switch type="decision">
    	<opt id="0">
        	<opttext>Get the banana</opttext>
            
            <text char="Queen">Potassium</text>
        </opt>
        
        <opt id="1">
        	<opttext>Do not get the banana</opttext>
            
            <text char="Queen">Kris, You're Gonna Get Sick.</text>
        </opt>
    </switch>
</story>
```

The initial story then has a `CurrentNode` of type `Phantonia.BranchingStory.TextNode` with the `Text` property set to `"Kris, Get The Banana"` and `Attributes["char"]` set to `"Queen"`.

Now call `Progress` on the story. You get a new story with a  `CurrentNode` of type `Phantonia.BranchingStory.SwitchNode`. Now you cannot call `Progress` because we have no idea with which option you wanna continue. Instead, call `ProgressWithOption` where you enter `0` or `1` as the argument. This tells us which of the different branches you want to take. For example, if you want to get the banana, call `ProgressWithOption(1)`.

Both node classes as well as the `Story` class are deeply immutable so any change such as progress creates a new instance.

Disclaimer: The library is a work in progress and not in any way finished.

