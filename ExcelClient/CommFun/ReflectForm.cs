using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

namespace ExcelClient
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ReflectForm
    {
        public ReflectForm()
        {
        }
        private object obj = null;
        private System.Type objType;
        public ReflectForm(string Path,string ClassName)
        {
            System.Reflection.Assembly ass;            
            ass = System.Reflection.Assembly.LoadFile(Path);
            objType = ass.GetType(ClassName);
            //objType = System.Type.GetType(ClassName);          
            obj = ass.CreateInstance(ClassName); 
        }
		public ReflectForm(string Path,string ClassName,object[] args){
			System.Reflection.Assembly ass;
			ass = System.Reflection.Assembly.LoadFile(Path);
			objType = ass.GetType(ClassName);
			if (args != null)
			{
				System.Type[] argtypes=new Type[args.Length];
				for(int i=0;i<args.Length;i++){
					argtypes[i]= args[i].GetType();                    
				}
			
				ConstructorInfo ci = objType.GetConstructor(argtypes);
				obj = ci.Invoke(args); 
				//obj = ass.CreateInstance(ClassName, true, BindingFlags.CreateInstance, null, args, null, null);
			}
			else{
				obj = System.Reflection.Assembly.GetAssembly(objType).CreateInstance(ClassName);
			}
		}
        public void ReflectWinForm(string Path, string ClassName,string parm, string arg)
        {
            string[] parms = parm.Split(',');
            string[] args = arg.Split(',');
            string info = "1";
            try
            {
                System.Reflection.Assembly ass;
                ass = System.Reflection.Assembly.LoadFile(Path);
                info += "-=-=2";
                objType = ass.GetType(ClassName);
                info += "-=-=3";
                obj = ass.CreateInstance(ClassName);
                info += "-=-=4";
                if (parms != null && args != null && parms.Length == args.Length)
                {
                    for (int i = 0; i < parms.Length; i++)
                        SetProperty(parms[i],args[i]);// Convert.ToString(args[i])
                }

                info += "-=-=5";
                Form frm = obj as Form;
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                throw new Exception(info+"--------------"+ex.Message+"==="+ex.StackTrace
                    );
            }
        }
        public void SetProperty(string propertyName, string propertyValue)
        {
            //设置属性值
            objType.InvokeMember(propertyName, BindingFlags.SetProperty, null, obj, new object[] { propertyValue });
        }

        //public Form returnForm()
        //{
        //    Form frm = obj as Form;
        //    return frm;
        //}
        public object returnFormObject()
        {
            return obj;
        }
        /*psPath= psPath+"\\bin\\"+dt.Rows[0]["JTSPDY_DLL"].ToString();
				//反射实现
				System.Reflection.Assembly ass;
				System.Type type ;
				string[] paramArr=new string[2]{psYWID,psSPJL};			 
				object obj;
			 
				ass = System.Reflection.Assembly.LoadFile(psPath);
				type = ass.GetType(dt.Rows[0]["JTSPDY_NSCLASS"].ToString());//必须使用名称空间+类名称
				System.Reflection.MethodInfo method = type.GetMethod(dt.Rows[0]["JTSPDY_MOTHED"].ToString());//方法的名称
				
				if(dt.Rows[0]["JTSPDY_ISSTATIC"].ToString()=="1")
				{
					errinfo= (string) method.Invoke(null, paramArr); //静态方法的调用
				}
				else
				{
					obj = ass.CreateInstance(dt.Rows[0]["JTSPDY_NSCLASS"].ToString());//必须使用名称空间+类名称
					errinfo=(string)method.Invoke(obj,paramArr); //实例方法的调用					
				} 
				method = null; 
				if(errinfo!=null && errinfo!="") throw new Exception(errinfo);*/
        //private string 


    }
}
