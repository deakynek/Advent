using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class SnailMath
    {
        List<string> numbers;

        public SnailMath(List<string> lines)
        {
            numbers = lines;

            var result = new LinkedTuple(lines[0].Trim(),0,false);
            foreach(var next in lines.Skip(1))
            {
                result = new LinkedTuple(result, next.Trim());

                while(result.CanExplode() || result.CanSplit())
                {
                    if(result.CanExplode())
                        result.DoExplode();
                    else if(result.CanSplit())
                        result.DoSplit();

                    
                    Console.WriteLine("\t"+result.GetStr());
                }
                Console.WriteLine(result.GetStr());
            }

            Console.WriteLine(result.GetValue());


            long max = 0;
            foreach (var line in lines)
            {
                foreach(var line2 in lines)
                {
                    if(line == line2)
                        continue;

                    var thisLine = new LinkedTuple(line, 0, false);
                    var added = new LinkedTuple(thisLine, line2);
                    while(added.CanExplode() || added.CanSplit())
                    {
                        if(added.CanExplode())
                            added.DoExplode();
                        else if(added.CanSplit())
                            added.DoSplit();
                    }   

                    var thisVal = added.GetValue();
                    if(thisVal> max)
                    {
                        max = thisVal;
                    }
                }
            }

            Console.WriteLine(max);
        }


    }

    class LinkedTuple
    {

        LinkedTuple left = null;
        LinkedTuple right = null;
        int level = 0;
        LinkedTuple parent = null;

        int? leftInt = null;
        int? rightInt = null;     

        bool isLeft;
    
        public LinkedTuple(string input,int depthLevel,  bool isLeft,LinkedTuple upper = null)
        {
            if(upper != null)
                parent = upper;

            level = depthLevel;
            if(level != 0)
                this.isLeft = isLeft;

            var splitIndex = -1;
            var runningLevel = 0;
            for(int i= 0; i<input.Length; i++)
            {
                if(input[i]=='[')
                    runningLevel +=1;
                else if(input[i]==']')
                    runningLevel -=1;
                else if(runningLevel == 1 && input[i] == ',')
                {
                    splitIndex = i;
                    break;
                }    
            }

            var leftStr = input.Substring(1,splitIndex-1);
            var rightStr = input.Substring(splitIndex+1);
            rightStr = rightStr.Substring(0, rightStr.Length-1);

            if(leftStr.StartsWith('['))
                left = new LinkedTuple(leftStr, level+1,true,this);
            else
                leftInt = int.Parse(leftStr);

            if(rightStr.StartsWith('['))
                right = new LinkedTuple(rightStr, level+1,false,this);
            else
                rightInt = int.Parse(rightStr);
        }

        public LinkedTuple(LinkedTuple existing, string adder)
        {
            level = 0;
            left = existing;
            left.isLeft = true;
            left.addLevel();
            left.parent = this;

            right = new LinkedTuple(adder, 1, false, this);
        }

        private void addLevel()
        {
            this.level += 1;
            if(left != null)
                left.addLevel();
            if(right != null)
                right.addLevel();
        }

        public bool CanExplode()
        {
            if(left != null && left.CanExplode())
                return true;

            if(right !=null && right.CanExplode())
                return true;

            if(rightInt.HasValue && leftInt.HasValue && level>=4)
                return true;

            return false;
        }

        public bool CanSplit()
        {   
            if(leftInt.HasValue && leftInt.Value>=10 || 
                rightInt.HasValue && rightInt.Value>=10)
                return true;
            if(left != null && left.CanSplit())
                return true;

            if(right !=null && right.CanSplit())
                return true;



            return false; 
        }

        public bool DoSplit()
        {
            if(!this.CanSplit())
                return false;

            var text = "";
            if(leftInt.HasValue && leftInt.Value>=10)
            {
                text = String.Format("[{0},{1}]",leftInt.Value/2, leftInt.Value-leftInt.Value/2);

                left = new LinkedTuple(text, this.level+1, true, this);
                leftInt = null;
                return true;
            }
            else if(left!= null && left.CanSplit())
            {
                left.DoSplit();
                return true;
            }
            else if(right != null && right.CanSplit())
            {
                right.DoSplit();
                return true;
            }
            else if(rightInt.HasValue && rightInt.Value>=10)
            {
                text = String.Format("[{0},{1}]",rightInt.Value/2, rightInt.Value-rightInt.Value/2);

                right = new LinkedTuple(text, this.level+1, false, this);
                rightInt = null;
                return true;
            }
            return false;

        }

        public bool DoExplode()
        {
            if(!this.CanExplode())
                return false;

            if(left!= null && left.CanExplode())
            {
                left.DoExplode();
                leftInt = 0;
                return true;
            }
            if(right != null && right.CanExplode())
            {
                right.DoExplode();
                rightInt  = 0;
                return true;
            }

            if(rightInt.HasValue && leftInt.HasValue)
            {


                addLeftUp(leftInt.Value);
                addRightUp(rightInt.Value);
                if(isLeft)
                {
                    parent.left = null;
                }
                else
                {
                    parent.right = null;
                }
                
                return true;
            }

            return false;
        }

        private void addLeftUp(int explodeLeft)
        {
            if(isLeft && parent!= null)
            {
                parent.addLeftUp(explodeLeft);
            }
            else if(!isLeft && parent!= null)
            {
                if(parent.left!= null)
                    parent.left.addRightDown(explodeLeft);
                else if(parent.leftInt != null)
                    parent.leftInt += explodeLeft;
            }
        }

        private void addLeftDown(int value)
        {
            if(left != null)
            {
                left.addLeftDown(value);
            }
            else if(leftInt.HasValue)
            {
                leftInt += value;
            }
        }
        private void addRightDown(int value)
        {
            if(right!=null)
            {
                right.addRightDown(value);
            }
            else if(rightInt.HasValue)
            {
                rightInt += value;
            }
        }

        private void addRightUp(int explodeRight)
        {
            if(!isLeft && parent!= null)
            {
                parent.addRightUp(explodeRight);
            }
            else if(isLeft&& parent!= null)
            {
                if(parent.right!= null)
                    parent.right.addLeftDown(explodeRight);
                else if(parent.rightInt != null)
                    parent.rightInt += explodeRight;
            }
        }

        public string GetStr()
        {
            var leftStr = leftInt.ToString();
            if(left != null)
                leftStr = left.GetStr();
            
            var rightStr = rightInt.ToString();
            if(right !=null)
                rightStr = right.GetStr();

            return String.Format("[{0},{1}]", leftStr, rightStr);
        }

        public long GetValue()
        {
            if(left == null && right ==null)
                return (long)(leftInt.Value*3 + rightInt.Value*2);

            var lowerLeft = (long)0;
            if(left!= null)
                lowerLeft = left.GetValue();
            else if(leftInt.HasValue)
                lowerLeft = leftInt.Value;

            var lowerRight = (long)0;
            if(right != null)
                lowerRight = right.GetValue();
            else if(rightInt.HasValue)
                lowerRight = rightInt.Value;

            return lowerLeft*3+ lowerRight*2;

        }
    }    
}