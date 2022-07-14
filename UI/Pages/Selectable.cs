﻿namespace UI.Pages;

public class Selectable : Selectable<string> { }

public class Selectable<T>
{
    public T? Name { get; set; }
    public bool Selected { get; set; }
}