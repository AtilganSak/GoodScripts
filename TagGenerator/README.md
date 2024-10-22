Firstly put the TagGenerator file in an Editor folder.

Go to 'Tools/Generate Tags' press and generate tags. 

After it will be created Tags.cs file in the 'Assets/_GameAssets/Scripts/Tags.cs'

You can change the URL from the TagsGenerator.cs
``` C#
32 - string filePath = "Assets/_GameAssets/Scripts/Tags.cs";
```

# Usage

``` C#
    if (other.CompareTag(Tags.DamageCollider))
    {
        \\DO something
    }
```
