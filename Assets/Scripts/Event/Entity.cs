/// <summary>
/// 値型を参照型にするためのラッパークラス
/// 別に値型じゃなくてもいいけど
/// </summary>
/// <typeparam name="T"></typeparam>
public class Entity<T> {
    private T _value;
    public T Value {
        get => _value;
        set => _value = value;
    }
    public Entity(T value) {
        _value = value;
    }
}
