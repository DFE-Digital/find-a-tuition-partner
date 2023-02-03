namespace Application.Common.Models
{
    public class Selectable : Selectable<string> { }

    public class Selectable<T>
    {
        public T Name { get; set; } = default!;
        public bool Selected { get; set; }
    }
}
