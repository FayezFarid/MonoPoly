using System;


public struct Index
{
    private int _index_Primitive;
    private int _maxLength;
    private bool _reLoopIndex;

    public int CurrentIndex => _index_Primitive;

    public int MaxLength
    {
        get { return _maxLength; }
        set
        {
            _maxLength = value-1;
            if (_index_Primitive == value)
            {
                _index_Primitive = 0;
            }
        }
    }

    public void MoveNextIndex()
    {
        int expectedNextIndex = Extension.GetNextIndex(_index_Primitive, _maxLength);
        if (!_reLoopIndex && expectedNextIndex < _index_Primitive)
        {
            return;
        }

        _index_Primitive = expectedNextIndex;
        // return _index_Primitive;
    }

    public int NextIndex
    {
        get
        {
            MoveNextIndex();
            return _index_Primitive;
        }
    }

    public int GetPrevious
    {
        get
        {
            if (_index_Primitive == 0)
            {
                return _maxLength;
            }

            return _index_Primitive - 1;
        }
    }

    public void GoPrevious()
    {
        if (_index_Primitive == 0)
        {
            _index_Primitive = _maxLength;
            return;
        }

        _index_Primitive -= 1;
    }

    public Index(int startingIndex, int maxLength, bool reLoopIndex)
    {
        _maxLength = maxLength - 1;
        _index_Primitive = startingIndex;
        _reLoopIndex = reLoopIndex;
    }
}