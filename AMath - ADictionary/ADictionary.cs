namespace System.Collections.Generic
{

    public class ADictionary<T1, T2> : IEnumerable
    {
        public List<AKeyValuePair<T1, T2>> Pairs = new List<AKeyValuePair<T1, T2>>();

        bool m_AllowSameKey;
        bool m_AllowSameValue;
        int m_LimitForKey;
        int m_LimitForValue;

        public ADictionary(bool _allowSameKey = true, bool _allowSameValue = true, int _limitForKey = 0, int _limitForValue = 0)
        {
            m_AllowSameKey = _allowSameKey;
            m_AllowSameValue = _allowSameValue;
            m_LimitForKey = _limitForKey;
            m_LimitForValue = _limitForValue;
        }

        public bool Add(AKeyValuePair<T1, T2> _aKeyValuePair)
        {
            Pairs.Add(_aKeyValuePair);
            return true;
        }
        public bool AddKey(T1 key, T2 value)
        {
            if(!m_AllowSameKey)
            {
                if(ContainsKey(key))
                {
                    return false;
                }
            }
            if(m_LimitForKey > 0)
            {
                if(KeyCount() >= m_LimitForKey)
                {
                    return false;
                }
            }
            AKeyValuePair<T1, T2> newKeyValue = new AKeyValuePair<T1, T2>();
            newKeyValue.Key = key;
            newKeyValue.Values = new List<T2>();
            newKeyValue.Values.Add(value);
            Pairs.Add(newKeyValue);
            return true;
        }
        public bool AddValue(T1 toKey, T2 value)
        {
            for(int i = 0; i < Pairs.Count; i++)
            {
                if(Pairs[i].Key.Equals(toKey))
                {
                    if(Pairs[i].Values != null)
                    {
                        if(!m_AllowSameValue)
                        {
                            if(Pairs[i].Values.Contains(value))
                            {
                                return false;
                            }
                        }
                        if(m_LimitForValue > 0)
                        {
                            if(ValueCount(toKey) >= m_LimitForValue)
                            {
                                return false;
                            }
                        }
                        Pairs[i].Values.Add(value);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool RemoveKey(T1 key)
        {
            for(int i = 0; i < Pairs.Count; i++)
            {
                if(Pairs[i].Key.Equals(key))
                {
                    Pairs.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
        public bool RemoveValue(T2 value)
        {
            bool rs = false;
            for(int i = 0; i < Pairs.Count; i++)
            {
                for(int k = 0; k < Pairs[i].Values.Count; k++)
                {
                    if(Pairs[i].Values[k].Equals(value))
                    {
                        Pairs[i].Values.RemoveAt(k);
                        rs = true;
                    }
                }
            }
            return rs;
        }
        public bool RemoveValue(T1 targetKey, T2 value)
        {
            for(int i = 0; i < Pairs.Count; i++)
            {
                if(Pairs[i].Key.Equals(targetKey))
                {
                    if(Pairs[i].Values != null)
                    {
                        if(Pairs[i].Values.Contains(value))
                        {
                            Pairs[i].Values.Remove(value);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool ContainsKey(T1 key)
        {
            for(int i = 0; i < Pairs.Count; i++)
            {
                if(Pairs[i].Key.Equals(key))
                {
                    return true;
                }
            }
            return false;
        }
        public bool ContainsValue(T1 targetKey, T2 value)
        {
            for(int i = 0; i < Pairs.Count; i++)
            {
                if(Pairs[i].Key.Equals(targetKey))
                {
                    for(int k = 0; k < Pairs[i].Values.Count; k++)
                    {
                        if(Pairs[i].Values[k].Equals(value))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public int KeyCount() => Pairs.Count;
        public int ValueCount()
        {
            int totalValueCount = 0;
            for(int i = 0; i < Pairs.Count; i++)
            {
                if(Pairs[i].Key != null)
                {
                    if(Pairs[i].Values != null)
                    {
                        totalValueCount += Pairs[i].Values.Count;
                    }
                }
            }
            return totalValueCount;
        }
        public int ValueCount(T1 key)
        {
            int totalValueCount = 0;
            for(int i = 0; i < Pairs.Count; i++)
            {
                if(Pairs[i].Key.Equals(key))
                {
                    if(Pairs[i].Values != null)
                    {
                        totalValueCount += Pairs[i].Values.Count;
                    }
                }
            }
            return totalValueCount;
        }

        public AKeyValuePair<T1, T2> this[int index]
        {
            get
            {
                if(index < Pairs.Count && index >= 0)
                    return Pairs[index];
                else
                    return default;
            }
            set
            {
                if(index < Pairs.Count && index >= 0)
                {
                    Pairs[index] = value;
                }
            }
        }
        public AKeyValuePair<T1, T2> this[T1 key]
        {
            get
            {
                return Pairs.Find(x => x.Key.Equals(key));
            }
            set
            {
                for(int i = 0; i < Pairs.Count; i++)
                {
                    if(Pairs[i].Key.Equals(key))
                    {
                        Pairs[i] = value;
                        break;
                    }
                }
            }
        }

        public void ChangeKey(T1 _oldKey, T1 _newKey)
        {
            if(!m_AllowSameKey)
            {
                if(ContainsKey(_newKey))
                {
                    return;
                }
            }
            for(int i = 0; i < Pairs.Count; i++)
            {
                if(Pairs[i].Key.Equals(_oldKey))
                {
                    AKeyValuePair<T1, T2> tmpAVP = Pairs[i];
                    tmpAVP.Key = _newKey;
                    Pairs[i] = tmpAVP;
                }
            }
        }
        public void ChangeKey(int _keyIndex, T1 _newKey)
        {
            if(!m_AllowSameKey)
            {
                if(ContainsKey(_newKey))
                {
                    return;
                }
            }
            AKeyValuePair<T1, T2> tmpAVP = Pairs[_keyIndex];
            tmpAVP.Key = _newKey;
            Pairs[_keyIndex] = tmpAVP;
        }
        public void ChangeValue(T1 _key, T2 _oldValue, T2 _newValue)
        {
            for(int i = 0; i < Pairs.Count; i++)
            {
                if(Pairs[i].Key.Equals(_key))
                {
                    if(Pairs[i].Values != null)
                    {
                        if(!m_AllowSameValue)
                        {
                            if(ContainsValue(_key, _newValue))
                            {
                                return;
                            }
                        }
                        for(int k = 0; k < Pairs[i].Values.Count; k++)
                        {
                            if(Pairs[i].Values[k].Equals(_oldValue))
                            {
                                Pairs[i].Values[k] = _newValue;
                            }
                        }
                    }
                }
            }
        }
        public void ChangeValue(int _keyIndex, int _valueIndex, T2 _newValue)
        {
            if(!m_AllowSameValue)
            {
                if(ContainsValue(Pairs[_keyIndex].Key, _newValue))
                {
                    return;
                }
            }
            AKeyValuePair<T1, T2> tmpAVP = Pairs[_keyIndex];
            tmpAVP.Values[_valueIndex] = _newValue;
            Pairs[_keyIndex] = tmpAVP;
        }

        public void ReverseKeys()
        {
            Pairs.Reverse();
        }
        public void ReverseValues(T1 _key)
        {
            for(int i = 0; i < Pairs.Count; i++)
            {
                if(Pairs[i].Key.Equals(_key))
                {
                    Pairs[i].Reverse();
                }
            }
        }
        public void ReverseValues(int _keyIndex)
        {
            Pairs[_keyIndex].Reverse();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Pairs.GetEnumerator();
        }
    }
    [System.Serializable]
    public struct AKeyValuePair<T1, T2>
    {
        public T1 Key;
        public List<T2> Values;

        public T2 this[int index]
        {
            get
            {
                if(index < Values.Count)
                    return Values[index];
                else
                    return default;
            }
            set
            {
                if(index < Values.Count)
                    Values[index] = value;
            }
        }
        public void Reverse()
        {
            if(Values != null)
            {
                Values.Reverse();
            }
        }
    }
}