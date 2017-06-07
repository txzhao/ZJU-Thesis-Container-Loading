function BorderVar = BorderVarCompute(Border)
WBox = max(size(Border,1),size(Border,2));

for i = 1 : WBox - 2
    Border_center(i) = Border(i+1);
end

flag = 0;
BorderVar = std(Border_center,flag);

end
