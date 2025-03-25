using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileCleanInfo
{
    public class FileClean
    {
        static List<FileInfo> _allFiles { get; set; }
        static List<string> _tags;
        static List<string> _attributes;
        static List<string> _functions;
        static List<Regex> _customs;
        enum Type
        {
            tag,
            attribute,
            function,
            custom
        }
        static FileClean()
        {
            _tags = new List<string> { "parameter name=\"resident_cntr\" type=\"VARCHAR2\"", "parameter name=\"contacts\" type=\"VARCHAR2\"", "parameter name=\"ppl_givnames\" type=\"VARCHAR2\"", "parameter name=\"sluzh_info\" type=\"VARCHAR2\"", "parameter name=\"stringEntity\" type=\"VARCHAR2\"", "parameter name=\"ppl_surname\" type=\"VARCHAR2\"", "parameter name=\"ppl_sex\" type=\"VARCHAR2\"", "parameter name=\"ppl_citizenship\" type=\"VARCHAR2\"", "parameter name=\"workplaces\" type=\"VARCHAR2\"", "parameter name=\"visa\" type=\"VARCHAR2\"", "parameter name=\"cntr_idn\" type=\"VARCHAR2\"", "parameter name=\"docs\" type=\"VARCHAR2\"", "parameter name=\"cust_idn\" type=\"VARCHAR2\"", "parameter name=\"user_change\" type=\"VARCHAR2\"", "op:Country", "op:PersonalNumber", "gc:PersonalNumber", "gc:LastName", "gc:FirstName", "gc:MiddleName", "gc:BirthDate", "gc:IsResident", "gc:CitizenshipCode", "gc:SexData", "gc:DocNumber", "gc:DocOrgan", "gc:DocDateIssue", "gc:DocDateExpiry", "gc:DocStatus", "gc:DocNumberIdBy", "gc:PersonalNumberOther", "gc:Note", "gc:CountryCode", "gc:Addresses", "gc:Contacts", "loginData", "firstName", "lastName", "birthDate", "passNo", "value", "ax24:matchedWord", "ax24:searchedWord", "Data", "EngPage", "RusPage" };
            _attributes = new List<string> { "ppl_reg_index", "ppl_reg_region", "ppl_kor_town", "ppl_doc_type", "ppl_kor_building", "ppl_reg_cntr", "ppl_home_streettype", "ppl_birth_cntr", "ppl_kor_house", "ppl_doc_cntr", "ppl_doc_place", "ppl_reg_streettype", "ppl_doc_organ_biom", "ppl_children_num", "ppl_kor_street", "ppl_birth_towntype", "ppl_birth_building", "ppl_birth_town", "ppl_home_house", "ppl_home_room", "ppl_givnames", "ppl_lastname", "ppl_surname", "ppl_reg_town", "ppl_kor_cntr", "ppl_home_street", "ppl_home_towntype", "ppl_citizenship", "ppl_home_region", "ppl_doc_nr", "ppl_birth_street", "ppl_home_subregion", "ppl_birth_house", "ppl_birth_date", "ppl_home_building", "ppl_vat_code", "erkContacts", "ppl_mmsurn", "ppl_kor_subregion", "ppl_birth_streettype", "workPlaces", "ppl_kor_room", "ppl_birth_region", "ppl_birth_subregion", "ppl_birth_room", "ppl_home_town", "ppl_perscode", "ppl_doc_dt", "ppl_home_index", "ppl_doc_expired_dt", "ppl_reg_house", "ppl_sex", "ppl_kor_towntype", "ppl_kor_streettype", "ppl_kor_index", "ppl_reg_room", "ppl_reg_subregion", "ppl_reg_towntype", "ppl_reg_building", "visa", "ppl_home_cntr", "ppl_birth_index", "ppl_reg_street", "ppl_kor_region", "marital_status", "latin_surname", "parameters", "personData", "doc_type", "doc_nr", "doc_place", "doc_organ_biom", "doc_cntr", "doc_dt", "doc_expired_dt", "doc_pers_num", "doc_status", "indAuthor", "changeAuthor", "erkCode", "sprType", "sprCode", "sprValue", "idRecord", "latin_givnames", "erkDocs", "erkWorkplaces", "vip", "resident_cntr", "significant", "doc_id" };
            _functions = new List<string> { "getActualDocumentInfo", "getWorkplacesInfo", "getContactsInfo", "getVisaInfo" };
            _customs = new List<Regex> { new Regex("Forpost client search Data: {(?s)(.*?)}"), new Regex("Получены следующие данные.(?s)(.*?)\r"), new Regex("value=ObjectProperty \\[value: (?s)(.*?)\\]"), new Regex(@"docNr: (?s)(.*?)\r"), new Regex("Документ с формы: (?s)(.*?)\r"), new Regex("xmlMap: (?s)(.*?)\r"), new Regex("Тело запроса для DataAnswer: (?s)(.*?)\r"), new Regex("Результат сканирования документа: (?s)(.*?)\r") };
        }
        public static List<string> Tags
        {
            get { return _tags; }
            set { _tags = value; }
        }

        public static List<string> Attributes
        {
            get { return _attributes; }
            set { _attributes = value; }
        }
        public static List<string> Functions
        {
            get { return _functions; }
            set { _functions = value; }
        }
        public static List<Regex> Customs
        {
            get { return _customs; }
            set { _customs = value; }
        }

        public static void GetAllFiles()
        {
            _allFiles = Directories.GetFiles();
            foreach (var file in _allFiles)
            {
                StringBuilder fileData = new StringBuilder();
                using (StreamReader reader = new StreamReader(file.FullName, Encoding.Default))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        fileData.AppendLine(line);
                    }
                }

                foreach (var custom in _customs)
                    IndexesSearch(null, custom, fileData, Type.custom);

                foreach (var function in _functions)
                    IndexesSearch(function, null, fileData, Type.function);

                foreach (var attribute in _attributes)
                    IndexesSearch(attribute, null, fileData, Type.attribute);

                foreach (var tag in _tags)
                    IndexesSearch(tag, null, fileData, Type.tag);


                using (StreamWriter writer = new StreamWriter(Directories.GetOutput() + file.Name))
                {
                    writer.Write(fileData.ToString());
                }
            }
        }
        static void IndexesSearch(string tagOpen, Regex reg, StringBuilder fileData, Type type, string tagClose = "", int index = 0)
        {
            List<string> attributeWrappers = new List<string> { "\"\"", "{}", "[]", null };

            switch (type)
            {
                case Type.tag:
                    tagOpen = $"<{tagOpen}>";
                    if (tagOpen.Contains(" "))
                        tagClose = tagOpen.Substring(0, tagOpen.IndexOf(" ")).Insert(1, "/") + ">";
                    else
                        tagClose = tagOpen.Insert(1, "/");
                    SearchCycle(fileData, Type.tag, index, tagOpen, tagClose);
                    break;
                case Type.attribute:
                    foreach (var wrapper in attributeWrappers)
                    {
                        if (wrapper == null || wrapper.Length < 2)
                            SearchCycle(fileData, Type.attribute, index, $"{tagOpen}=", tagClose);
                        else
                            SearchCycle(fileData, Type.attribute, index, $"{tagOpen}={wrapper[0].ToString()}", wrapper[1].ToString());
                    }
                    break;
                case Type.function:
                    tagOpen = $"{tagOpen}(";
                    tagClose = ")";
                    SearchCycle(fileData, Type.function, index, tagOpen, tagClose);
                    break;
                case Type.custom:
                    SearchCycle(fileData, Type.custom, index, reg: reg);
                    break;
            }
        }
        static void SearchCycle(StringBuilder fileData, Type type, int index, string tagOpen = "", string tagClose = "", Regex reg = null)
        {
            int indexDifferenceSearchData = 0;
            Regex valueReg = new Regex("(?s)(.*?)");
            while (true)
            {
                index = fileData.ToString().IndexOf(tagOpen, index);
                if (index < 0)
                    break;
                switch (type)
                {
                    case Type.tag:
                    case Type.function:
                        fileData.Remove(index + tagOpen.Length, fileData.ToString().IndexOf(tagClose, index + tagOpen.Length) - (index + tagOpen.Length));
                        break;
                    case Type.attribute:
                        var a = fileData.ToString().Substring(index, fileData.ToString().IndexOf(tagClose, index + tagOpen.Length) + tagClose.Length - index);
                        fileData.Remove(index, fileData.ToString().IndexOf(tagClose, index + tagOpen.Length) + tagClose.Length - index);
                        CleaningAfterAnAttributtes(false, index, fileData);
                        break;
                    case Type.custom:
                        Match match = reg.Match(fileData.ToString(), index);
                        if (match.Value.Length == 0)
                            return;
                        string patternRegBeginning = Regex.Unescape(reg.ToString().Substring(0, reg.ToString().IndexOf(valueReg.ToString())));
                        string patternRegEnding = Regex.Unescape(reg.ToString().Substring(reg.ToString().IndexOf(valueReg.ToString()) + valueReg.ToString().Length, reg.ToString().Length - (reg.ToString().IndexOf(valueReg.ToString()) + valueReg.ToString().Length)));
                        int customRemoveBeginIndex = match.Index + patternRegBeginning.Length - indexDifferenceSearchData;
                        int customLength = match.Value.Length - (patternRegBeginning.Length + patternRegEnding.Length);

                        if (customLength > 0)
                        {
                            string customRemove = match.Value.Substring(patternRegBeginning.Length, match.Value.Length - (patternRegBeginning.Length + patternRegEnding.Length));
                            fileData.Remove(customRemoveBeginIndex + indexDifferenceSearchData, customRemove.Length);
                            indexDifferenceSearchData = customRemove.Length;
                        }
                        index = index + match.Value.Length;
                        break;
                }
                index += tagClose.Length;
            }
        }
        public static void CleaningAfterAnAttributtes(bool forRight, int index, StringBuilder fileData, int counter = 0)
        {
            List<string> forCleanAfterRemove = new List<string> { " ", "," };
            counter = index;

            if (forRight == true)
            {
                while (true)
                {
                    if (forCleanAfterRemove.Contains(fileData.ToString().Substring(counter + 1, 1)) == true)
                    {
                        fileData.Remove(counter + 1, 1);
                        counter++;
                    }
                    else
                        break;
                }
            }
            else
            {
                while (true)
                {
                    if (forCleanAfterRemove.Contains(fileData.ToString().Substring(counter - 1, 1)) == true)
                    {
                        fileData.Remove(counter - 1, 1);
                        counter++;
                    }
                    else
                    {
                        if (forRight == false)
                            CleaningAfterAnAttributtes(true, index, fileData);
                        break;
                    }
                }
            }
        }
        public static List<string> ListInitialize(List<string> xmlList)
        {
            return xmlList;
        }
    }
}