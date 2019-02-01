num1 = currentArgs[1]
operator = currentArgs[2]
num2 = currentArgs[3]
sum = 0
if operator == "+" then
	sum = num1+num2
elseif operator == "*" then
	sum = num1*num2
elseif operator == "/" then
	sum = num1/num2
elseif operator == "-" then
	sum = num1-num2
end
Computer.WriteLine(sum)