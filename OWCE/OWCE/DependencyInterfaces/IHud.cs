using System;
namespace OWCE
{
    public interface IHud
    {
        void Show(string message);
        void Show(string message, float progress);
        void Show(string message, float progress, string cancel, Action cancelCallback);
        void Show(string message, string cancel, Action cancelCallback);
        void Dismiss();
    }
}
