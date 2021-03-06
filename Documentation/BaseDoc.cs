﻿namespace XmlDocToSlateMD.Documentation
{
    public class BaseDoc
    {
        public string Name;

        public BaseDoc Parent;

        public virtual object this [string param] {
            get {
                return this.GetFieldValue(param);
            }
            set {
                this.SetFieldValue(param, value);
            }
        }

        public BaseDoc(BaseDoc parent)
        {
            if (parent == null) {
                System.Console.WriteLine($"parent is <null> in {GetType()} constructor");
                return;
            }
            Parent = parent;
            parent.RegisterChild(this);
        }

        public virtual void RegisterChild(BaseDoc child)
        {
            throw new System.InvalidOperationException("RegisterChild is not supported on: " + GetType().AssemblyQualifiedName);
        }
    }
}

