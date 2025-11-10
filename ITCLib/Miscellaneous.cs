using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class VariableLabel
    {

        public int ID { get; set; }
        public string LabelText { get; set; }
        public int Uses { get; set; }

        public VariableLabel()
        {
            LabelText = string.Empty;
        }

        public VariableLabel(int id, string label)
        {
            ID = id;
            LabelText = label;
        }

        public VariableLabel(ContentLabel label)
        {
            ID = label.ID;
            LabelText = label.LabelText;
        }

        public VariableLabel(TopicLabel label)
        {
            ID = label.ID;
            LabelText = label.LabelText;
        }

        public VariableLabel(DomainLabel label)
        {
            ID = label.ID;
            LabelText = label.LabelText;
        }

        public VariableLabel(ProductLabel label)
        {
            ID = label.ID;
            LabelText = label.LabelText;
        }

        public override string ToString()
        {
            return LabelText;
        }

        public override bool Equals(object obj)
        {
            var label = obj as VariableLabel;
            return label != null &&
                   ID == label.ID &&
                   LabelText == label.LabelText;
        }

        public override int GetHashCode()
        {
            var hashCode = -244446586;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LabelText);
            return hashCode;
        }
    }

    public class DomainLabel : VariableLabel
    {
        public DomainLabel()
        {
            LabelText = string.Empty;
        }

        public DomainLabel(int id, string label)
        {
            ID = id;
            LabelText = label;
        }

        public DomainLabel(DomainLabel domain)
        {
            ID = domain.ID;
            LabelText = domain.LabelText;
        }

        public override string ToString()
        {
            return LabelText;
        }

        public override bool Equals(object obj)
        {
            var label = obj as DomainLabel;
            return label != null &&
                   ID == label.ID &&
                   LabelText == label.LabelText;
        }

        public override int GetHashCode()
        {
            var hashCode = -244446586;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LabelText);
            return hashCode;
        }
    }

    public class TopicLabel : VariableLabel
    {
        public TopicLabel()
        {
            LabelText = string.Empty;
        }

        public TopicLabel(int id, string label)
        {
            ID = id;
            LabelText = label;
        }
        public TopicLabel(TopicLabel topic)
        {
            ID = topic.ID;
            LabelText = topic.LabelText;
        }

        public override string ToString()
        {
            return LabelText;
        }

        public override bool Equals(object obj)
        {
            var label = obj as TopicLabel;
            return label != null &&
                   ID == label.ID &&
                   LabelText == label.LabelText;
        }

        public override int GetHashCode()
        {
            var hashCode = -244446586;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LabelText);
            return hashCode;
        }
    }

    public class ContentLabel : VariableLabel
    {
        public ContentLabel()
        {
            LabelText = string.Empty;
        }

        public ContentLabel(int id, string label)
        {
            ID = id;
            LabelText = label;
        }

        public ContentLabel(ContentLabel content)
        {
            ID = content.ID;
            LabelText = content.LabelText;
        }

        public override string ToString()
        {
            return LabelText;
        }

        public override bool Equals(object obj)
        {
            var label = obj as ContentLabel;
            return label != null &&
                   ID == label.ID &&
                   LabelText == label.LabelText;
        }

        public override int GetHashCode()
        {
            var hashCode = -244446586;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LabelText);
            return hashCode;
        }
    }

    public class ProductLabel : VariableLabel
    {
        public ProductLabel()
        {
            LabelText = string.Empty;
        }

        public ProductLabel(int id, string label)
        {
            ID = id;
            LabelText = label;
        }

        public ProductLabel(ProductLabel product)
        {
            ID = product.ID;
            LabelText = product.LabelText;
        }

        public override string ToString()
        {
            return LabelText;
        }

        public override bool Equals(object obj)
        {
            var label = obj as ProductLabel;
            return label != null &&
                   ID == label.ID &&
                   LabelText == label.LabelText;
        }

        public override int GetHashCode()
        {
            var hashCode = -244446586;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LabelText);
            return hashCode;
        }
    }

    public class ScreenedProduct
    {
        public int ID { get; set; }
        public string ProductName { get; set; }

        public ScreenedProduct()
        {
            ProductName = "";
        }

        public ScreenedProduct(int id, string label)
        {
            ID = id;
            ProductName = label;
        }

        public ScreenedProduct(ScreenedProduct product)
        {
            ID = product.ID;
            ProductName = product.ProductName;
        }

        public override string ToString()
        {
            return ProductName;
        }

        public override bool Equals(object obj)
        {
            var label = obj as ScreenedProduct;
            return label != null &&
                   ID == label.ID &&
                   ProductName == label.ProductName;
        }

        public override int GetHashCode()
        {
            var hashCode = -244446586;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ProductName);
            return hashCode;
        }
    }

    public class UserState
    {
        public int ID { get; set; }
        public string UserStateName { get; set; }

        public UserState()
        {
            UserStateName = "";
        }

        public UserState(int id, string label)
        {
            ID = id;
            UserStateName = label;
        }

        public UserState(UserState product)
        {
            ID = product.ID;
            UserStateName = product.UserStateName;
        }

        public override string ToString()
        {
            return UserStateName;
        }

        public override bool Equals(object obj)
        {
            var label = obj as UserState;
            return label != null &&
                   ID == label.ID &&
                   UserStateName == label.UserStateName;
        }

        public override int GetHashCode()
        {
            var hashCode = -244446586;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UserStateName);
            return hashCode;
        }
    }

    public class Keyword
    {
        public int ID { get; set; }
        public string LabelText { get; set; }

        public Keyword()
        {
            LabelText = string.Empty;
        }

        public Keyword(int id, string label)
        {
            ID = id;
            LabelText = label;
        }

        public Keyword(Keyword product)
        {
            ID = product.ID;
            LabelText = product.LabelText;
        }

        public override string ToString()
        {
            return LabelText;
        }

        public override bool Equals(object obj)
        {
            var label = obj as Keyword;
            return label != null &&
                   ID == label.ID &&
                   LabelText == label.LabelText;
        }

        public override int GetHashCode()
        {
            var hashCode = -244446586;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LabelText);
            return hashCode;
        }
    }

    public class StringPair
    {
        public string String1 { get; set; }
        public string String2 { get; set; }

        public StringPair(string string1)
        {
            String1 = string1;
        }

        public StringPair(string string1, string string2)
        {
            String1 = string1;
            String2 = string2;
        }
    }

    public class ReportSurveySelector
    {
        public int ID { get; set; }
        public string Display { get; set; }

        public ReportSurveySelector (int id, string display)
        {
            ID = id;
            Display = display;
        }
    }

}
