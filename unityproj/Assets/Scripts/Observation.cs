
using System.Collections;

public class Observation
{
    public enum dtype { Float, Int }
    private string mName;
    private dtype mDtype;
    private float mValue;

    public Observation(string name, dtype dt)
    {
        mName = name;
        mDtype = dt;
        mValue = 0;
    }

    public string name()
    {
        return mName;
    }

    public string datatype()
    {
        switch(mDtype)
        {
            case dtype.Float:   return "float";
            case dtype.Int:     return "int";
            default:            return "None";
        }
    }

    public float value()
    {
        return mValue;
    }

    public void set_value(float value)
    {
        mValue = value;
    }
}
