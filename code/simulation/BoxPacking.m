function [BoxStack,Border,dAreaRatio_best,dSpaceRatio_best,C_best, FinishedBox_best,minBlankPoints,...
	BorderVars,BoxPackingInfo_record] = BoxPacking(BoxAmount,minBorderVar)
% 初始参数说明
% WBox, HBox, LBox - 集装箱尺寸
% BoxAmount - 货物个数，n*12的矩阵，其中n为货物站的个数
% BoxSize - 货物规格（长和宽）
% BoxQuantum - 一列最多放几个货物
% BoxStack - 每种货物的堆数
% StackSum - 每站货物的总堆数
% Border - 集装箱格局中现存货物的最高边界

WBox = 234;
HBox = 589;
LBox = 239;
BoxSize = [53, 29; 53, 23; 43, 21; 35, 19; 29, 17; 26, 15; 23, 13; 21, 11; 20, 11; 18, 10; 15, 9; 13, 8];
BoxQuantum = [6, 8, 8, 10, 12, 13, 14, 17, 17, 19, 21, 26];
length = [53, 53, 43, 35, 29, 26, 23, 21, 20, 18, 15, 13];
width = [29, 23, 21, 19, 17, 15, 13, 11, 11, 10, 9, 8];
height = [39, 29, 29, 23, 19, 18, 17, 14, 14, 12, 11, 9];

% % 挑选合适的长宽
% for i = 1 : 12
%         wasteV(1) = LBox*width(i)*height(i)-floor(LBox/length(i))*length(i)*width(i)*height(i);
%         wasteV(2) = LBox*length(i)*height(i)-floor(LBox/width(i))*length(i)*width(i)*height(i);
%         wasteV(3) = LBox*width(i)*length(i)-floor(LBox/height(i))*length(i)*width(i)*height(i);
% %         wasteV(j,1) = floor(BoxAmount(j,i)/num1(j,i))*wa(1)+LBox*width(i)*height(i)-...
% %             (BoxAmount(j,i)-floor(BoxAmount(j,i)/num1(j,i))*num1(j,i))*length(i)*width(i)*height(i);
% %         wasteV(j,2) = floor(BoxAmount(j,i)/num2(j,i))*wa(2)+LBox*length(i)*height(i)-...
% %             (BoxAmount(j,i)-floor(BoxAmount(j,i)/num2(j,i))*num2(j,i))*length(i)*width(i)*height(i);
% %         wasteV(j,3) = floor(BoxAmount(j,i)/num3(j,i))*wa(3)+LBox*width(i)*length(i)-...
% %             (BoxAmount(j,i)-floor(BoxAmount(j,i)/num3(j,i))*num3(j,i))*length(i)*width(i)*height(i);        
%     if min(wasteV) == wasteV(1)
%             BoxSize(i, 1) = max(width(i), height(i));
%             BoxSize(i, 2) = min(width(i), height(i));
%             height(i) = length(i);
%     elseif min(wasteV) == wasteV(2)
%             BoxSize(i, 1) = max(length(i), height(i));
%             BoxSize(i, 2) = min(length(i), height(i));
%             height(i) = width(i);
%      else 
%             BoxSize(i, 1) = max(width(i), length(i));
%             BoxSize(i, 2) = min(width(i), length(i));
%     end 
% end


% BoxSize = [53, 39; 53, 29; 43, 29; 35, 23; 29, 19; 26, 18; 23, 17; 21, 14; 20, 14; 18, 12; 15, 11; 13, 9];
BoxQuantum = [floor(LBox/height(1)), floor(LBox/height(2)), floor(LBox/height(3)), floor(LBox/height(4)),...
    floor(LBox/height(5)), floor(LBox/height(6)), floor(LBox/height(7)), floor(LBox/height(8)), ...
    floor(LBox/height(9)), floor(LBox/height(10)), floor(LBox/height(11)), floor(LBox/height(12))];
BoxVolume = [BoxSize(1,1)*BoxSize(1,2)*LBox/BoxQuantum(1), BoxSize(2,1)*BoxSize(2,2)*LBox/BoxQuantum(2), BoxSize(3,1)*BoxSize(3,2)*LBox/BoxQuantum(3),...
    BoxSize(4,1)*BoxSize(4,2)*LBox/BoxQuantum(4), BoxSize(5,1)*BoxSize(5,2)*LBox/BoxQuantum(5), BoxSize(6,1)*BoxSize(6,2)*LBox/BoxQuantum(6), BoxSize(7,1)*BoxSize(7,2)*LBox/BoxQuantum(7),...
    BoxSize(8,1)*BoxSize(8,2)*LBox/BoxQuantum(8), BoxSize(9,1)*BoxSize(9,2)*LBox/BoxQuantum(9), BoxSize(10,1)*BoxSize(10,2)*LBox/BoxQuantum(10), BoxSize(11,1)*BoxSize(11,2)*LBox/BoxQuantum(11),...
    BoxSize(12,1)*BoxSize(12,2)*LBox/BoxQuantum(12)]; 

%求每站每一种箱子的堆数，BoxStack表示
stat_num = size(BoxAmount, 1);
Box_num = size(BoxAmount, 2);
BoxQuantum_ = [];
for i = 1 : stat_num
    BoxQuantum_ = [BoxQuantum_; BoxQuantum];
end
BoxStack = BoxAmount ./ BoxQuantum_;
BoxMod = mod(BoxAmount,BoxQuantum_);
BoxStack = ceil(BoxStack);
 
%求每站箱子的总堆数，StackSum表示，StackSum为1*n的矩阵
StackSum = sum(BoxStack');

% 初始化格局: 边界为2, 中间为0(表示为空)
C0 = zeros(HBox+2, WBox+2);
C0(1,:) = 2*ones(1,WBox+2);
C0(HBox+2,:) = C0(1,:);
C0(:,1) = 2*ones(HBox+2,1);
C0(:,WBox+2) = C0(:,1);

%设置边界Border，Border为1*（WBox+2）的矩阵，表示每一列货物的高度
Border = ones(1, WBox + 2);
Border(1) = HBox + 1;
Border(WBox + 2) = HBox + 1;
NextBorder = Border;

%设置初始角信息
Angle0 = [2; 2; 0]; %第一个角在左下侧，坐标为（2,2），角类型为0
Angle0 = [Angle0 [2; WBox + 1; 1]]; %第二个角在右下侧，坐标为（2,WBox + 1），角类型为1

dAreaRatio_best = 0;
C_best = C0;
Angle_best = Angle0;
FinishedBox_best = [];
BoxPackingInfo_record = [];
BorderVars = [];
% C_best = struct('n1',C0,'n2',C0,'n3',C0,'n4',C0,'n5',C0);
% Angle_best = struct('n1',Angle0,'n2',Angle0,'n3',Angle0,'n4',Angle0,'n5',Angle0);
% FinishedBox_best = struct('n1',[],'n2',[],'n3',[],'n4',[],'n5',[]);
% BoxPackingInfo_record = struct('n1',[],'n2',[],'n3',[],'n4',[],'n5',[]);

for station = 1 : stat_num
%     minBlankPoints = [10000000 10000000 10000000 10000000 10000000];
%     NextBorderVar = [10000000 10000000 10000000 10000000 10000000];
    minBlankPoints = 10000000;
%     minBorderVar = 10000;
    for i = 1 : Box_num
        if BoxStack(station, i) == 0
            continue;
        end
	    [C, Angle, FinishedBox, BlankPoints, NextBorder, BoxPackingInfo] = OptimalPacking(i, station, C0, Angle0, Border, WBox, HBox, BoxSize, BoxStack, StackSum);
        BorderVar = BorderVarCompute(NextBorder);
%          if BlankPoints < minBlankPoints && BorderVar < minBorderVar 
         if BlankPoints < minBlankPoints
            minBlankPoints = BlankPoints;
%             minBorderVar = BorderVar;
            C_best = C;
            Angle_best = Angle;
            FinishedBox_Current = FinishedBox;
            NextBorder_best = NextBorder;
            BoxPackingInfo_best = BoxPackingInfo;
            BorderVar_Current = BorderVar;

         end
    end
	C0 = C_update(C_best, Border, NextBorder_best, WBox);  %更新C0
	Border = NextBorder_best;  %更新边界
	Angle0 = Angle_update(Border, WBox);  %更新Angle0
	FinishedBox_best = [FinishedBox_best; FinishedBox_Current];  %更新已完成的货物数量
    BoxPackingInfo_record = [BoxPackingInfo_record BoxPackingInfo_best];
    BorderVars = [BorderVars BorderVar_Current];
end
C0(1,:) = 2*ones(1,WBox+2);
C0(HBox+2,:) = C0(1,:);
C0(:,1) = 2*ones(HBox+2,1);
C0(:,WBox+2) = C0(:,1);
C_best = C0;

%计算dAreaRatio_best
points = sum(sum(C_best > 0)) - sum(sum(C_best > 2));
points = points - 2 * HBox - 2 * WBox - 4;
pointsum = sum(sum(Border(2:WBox+1) - 1));
dAreaRatio_best = points / pointsum;
% dAreaRatio_best = points / (max(Border(2:WBox+1)) * WBox);

% 计算dSpaceRatio_best
BoxRemain = BoxStack - FinishedBox_best;
for j = 1:size(FinishedBox_best,1)
    for k = 1:size(FinishedBox_best,2) 
        if BoxRemain(j,k) ~= 0
            BoxMod(j,k) = BoxQuantum_(j,k);
        end
    end
end
BoxVolumeSum = sum(sum((FinishedBox_best - ones(size(FinishedBox_best,1),size(FinishedBox_best,2)))*...
    (BoxVolume.*BoxQuantum)')) + sum(sum(BoxMod.*[BoxVolume;BoxVolume;BoxVolume]));
SpaceVolume = 0;
for i = 2 : WBox + 1
    SpaceVolume = SpaceVolume + (Border(i))*1*LBox;
end
dSpaceRatio_best = BoxVolumeSum /SpaceVolume;

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%最优算法
function [C, Angle, FinishedBox, BlankPoints, NextBorder, BoxPackingInfo] = OptimalPacking(firstBoxNo, stepNo, C_initial, Angle_initial, Border_initial, WBox, HBox, BoxSize, BoxStack, StackSum)
C = C_initial;
Angle = Angle_initial;
FinishedBox = zeros(1, 12);
BlankPoints = 0;
NextBorder = Border_initial;
BoxPackingInfo=[];

%遍历搜索
for num = 1:StackSum(stepNo)
    Packing = 0;  %本步骤是否放置货物，0表示没有放置
	Degree = 10000;  %本步骤放置货物的穴度
	Area = 0;  %本步骤放置货物的面积
	BoxPlace = [];  %本步骤放置货物的位置信息
	BoxWidth = 0;  %本步骤放置货物的宽度(已考虑转置)
	BoxHeight = 0;  %本步骤放置货物的长度(已考虑转置)
	BoxNoBest = 0;  %本步骤放置货物的序号
	%按角搜索
	for AngleNo = 1:size(Angle, 2)
	    BoxNo_best = 0;  %该角放置最佳货物的货物序号
		Degree_best = 10000;  %该角放置最佳货物的穴度
		Area_best = 0;  %该角放置最佳货物的面积
		Type_best = 0;  %该角放置最佳货物的类型：0表示正放；1表示转置
		%按货物搜索
		for BoxNo = 1:12
		    %第一步放置fisrtBoxNo
            if num == 1 && BoxNo ~= firstBoxNo
			    continue;
            end
			%判断该类型的货物是否有剩余
			if FinishedBox(BoxNo) == BoxStack(stepNo, BoxNo)
			    continue
			end
			%计算货物正放情况
			point = [Angle(1, AngleNo), Angle(2, AngleNo)];
			if Angle(3, AngleNo) == 1
			    point(2) = point(2) - BoxSize(BoxNo, 1) + 1;
			end
			valid = ValidPlace(point, BoxSize(BoxNo, 1), BoxSize(BoxNo, 2), C, WBox, HBox);  %计算该角的位置是否能够放置该货物
			%如果货物能够放置，计算其是否最优
			if valid == 1
			    Box_degree = DegreeCompute(point, BoxSize(BoxNo, 1), BoxSize(BoxNo, 2), C);  %计算该角的位置放置该货物的穴度
				if Box_degree < Degree_best
				    Degree_best = Box_degree;
					Area_best = BoxSize(BoxNo, 1) * BoxSize(BoxNo, 2);
					BoxNo_best = BoxNo;
					Type_best = 0;
                elseif Box_degree == Degree_best && BoxSize(BoxNo, 1) * BoxSize(BoxNo, 2) > Area_best
				    Degree_best = Box_degree;
					Area_best = BoxSize(BoxNo, 1) * BoxSize(BoxNo, 2);
					BoxNo_best = BoxNo;
					Type_best = 0;
				end
			end
			%计算货物转置放置情况
			point = [Angle(1, AngleNo), Angle(2, AngleNo)];
			if Angle(3, AngleNo) == 1
			    point(2) = point(2) - BoxSize(BoxNo, 2) + 1;
			end
			valid = ValidPlace(point, BoxSize(BoxNo, 2), BoxSize(BoxNo, 1), C, WBox, HBox);  %计算该角的位置是否能够放置该货物
			%如果货物能够放置，计算其是否最优
			if valid == 1
			    Box_degree = DegreeCompute(point, BoxSize(BoxNo, 2), BoxSize(BoxNo, 1), C);  %计算该角的位置放置该货物的穴度
				if Box_degree < Degree_best
				    Degree_best = Box_degree;
					Area_best = BoxSize(BoxNo, 1) * BoxSize(BoxNo, 2);
					BoxNo_best = BoxNo;
					Type_best = 1;
				elseif Box_degree == Degree_best && BoxSize(BoxNo, 1) * BoxSize(BoxNo, 2) > Area_best
				    Degree_best = Box_degree;
					Area_best = BoxSize(BoxNo, 1) * BoxSize(BoxNo, 2);
					BoxNo_best = BoxNo;
					Type_best = 1;
				end
			end
			%搜索最优货物
            if BoxNo_best == 0
                break;
            else
			    Packing = 1;
                if (Degree_best < Degree) || (Degree_best == Degree && Area_best > Area)
				    Degree = Degree_best;
					Area = Area_best;
					BoxPlace = [Angle(1, AngleNo), Angle(2, AngleNo)];
                    BoxWidth = BoxSize(BoxNo, 1);
                    BoxHeight = BoxSize(BoxNo, 2);
					BoxNoBest = BoxNo_best;
                    if Angle(3, AngleNo) == 1
                        if Type_best == 0
						    BoxPlace(2) = BoxPlace(2) - BoxSize(BoxNo, 1) + 1;
						else
						    BoxPlace(2) = BoxPlace(2) - BoxSize(BoxNo, 2) + 1;
                        end
                    end
                    if Type_best == 1
                        BoxWidth = BoxSize(BoxNo, 2);
                        BoxHeight = BoxSize(BoxNo, 1);
                    end
                end
            end
		end
	end
	%放置最优货物
	if Packing == 0
	    continue;
	end
	FinishedBox(BoxNoBest) = FinishedBox(BoxNoBest) + 1;
	C(BoxPlace(1), BoxPlace(2):BoxPlace(2) + BoxWidth - 1) = 2;
	C(BoxPlace(1) + BoxHeight - 1, BoxPlace(2):BoxPlace(2) + BoxWidth - 1) = 2;
	C(BoxPlace(1):BoxPlace(1) + BoxHeight - 1, BoxPlace(2)) = 2;
	C(BoxPlace(1):BoxPlace(1) + BoxHeight - 1, BoxPlace(2) + BoxWidth - 1) = 2;
	C(BoxPlace(1) + 1:BoxPlace(1) + BoxHeight - 2, BoxPlace(2) + 1:BoxPlace(2) + BoxWidth - 2) = 1;
	%修改边界
	for k = BoxPlace(2):BoxPlace(2) + BoxWidth - 1
        if BoxPlace(1) + BoxHeight - 1 > NextBorder(k)
		    NextBorder(k) = BoxPlace(1) + BoxHeight - 1;
        end
	end
	%修改角
	%删除类型为0的角
	if C(BoxPlace(1), BoxPlace(2) - 1) > 0
        for k = 1:size(Angle, 2)
            if Angle(1, k) == BoxPlace(1) && Angle(2, k) == BoxPlace(2)
			    Angle(:, k) = [];
				break;
            end
        end
	end
	%删除类型为1的角
    if C(BoxPlace(1), BoxPlace(2) + BoxWidth) > 0
        for k = 1:size(Angle, 2)
            if Angle(1, k) == BoxPlace(1) && Angle(2, k) == BoxPlace(2) + BoxWidth - 1
                Angle(:, k) = [];
                break;
            end
        end
    end
	%添加新角
	%左右两边添加新角
	for k = BoxPlace(1):BoxPlace(1) + BoxHeight - 1
        if C(k, BoxPlace(2) - 1) == 0 && C(k - 1, BoxPlace(2) - 1) > 0
		    Angle = [Angle [k; BoxPlace(2) - 1; 1]];
        end
		if C(k, BoxPlace(2) + BoxWidth) == 0 && C(k - 1, BoxPlace(2) + BoxWidth) > 0
		    Angle = [Angle [k; BoxPlace(2) + BoxWidth; 0]];
		end
	end
	%上边添加新角
    for k = BoxPlace(2):BoxPlace(2) + BoxWidth - 1
        if C(BoxPlace(1) + BoxHeight, k) == 0 && C(BoxPlace(1) + BoxHeight, k - 1) > 0
		    Angle = [Angle [BoxPlace(1) + BoxHeight; k; 0]];
        elseif C(BoxPlace(1) + BoxHeight, k) == 0 && C(BoxPlace(1) + BoxHeight, k + 1) > 0
		    Angle = [Angle [BoxPlace(1) + BoxHeight; k; 1]];
        end
    end
    BoxPackingInfo=[BoxPackingInfo [BoxPlace(1);BoxPlace(2);BoxNoBest;Type_best]];
end
%计算BlankPoints
for k = 2:WBox + 1
    for j = Border_initial(k):NextBorder(k)
%     for j = 2:HBox + 1
        if C(j, k) == 0
		    BlankPoints = BlankPoints + 1;
        end
    end
end

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%计算新的C0
function C0 = C_update(C_best, Border, NextBorder_best, WBox)
C0 = C_best;
for k = 2:WBox + 1
    for j = Border(k):NextBorder_best(k)
        if C0(j, k) == 0
		    C0(j, k) = 3;
        end
    end
end


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%计算新的Angle0
function Angle0 = Angle_update(Border_information, WBox)
Angle0 = [];
number = 0;
for k = 2:WBox+1
    if Border_information(k) < Border_information(k - 1)
	    number = number + 1;
		Angle0 = [Angle0 [Border_information(k) + 1; k; 0]];
	elseif Border_information(k) < Border_information(k + 1)
	    number = number + 1;
		Angle0 = [Angle0 [Border_information(k) + 1; k; 1]];
    end
end

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%判断货物是否能否放置该处，返回1表示可以
function valid = ValidPlace(anglePlace, width, height, C_information, WBox, HBox)
valid = 0;
if anglePlace(2) < 2
    return
end
if anglePlace(2) + width - 1 > WBox + 1
    return
end
if anglePlace(1) + height - 1 > HBox + 1
    return
end
sum1 = sum(C_information(anglePlace(1), anglePlace(2):anglePlace(2) + width - 1));
sum2 = sum(C_information(anglePlace(1) + height - 1, anglePlace(2):anglePlace(2) + width - 1));
sum3 = sum(C_information(anglePlace(1):anglePlace(1) + height - 1, anglePlace(2)));
sum4 = sum(C_information(anglePlace(1):anglePlace(1) + height - 1, anglePlace(2) + width - 1));
if sum1 + sum2 + sum3 + sum4 > 0
    return
end
valid = 1;

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%计算货物摆放的穴度
function Box_degree = DegreeCompute(anglePlace, width, height, C_information)
Box_degree_left = 0;
Box_degree_right = 0;
while sum(C_information(anglePlace(1):anglePlace(1) + height - 1, anglePlace(2) - Box_degree_left - 1)) == 0
    Box_degree_left = Box_degree_left + 1;
end
while sum(C_information(anglePlace(1):anglePlace(1) + height - 1, anglePlace(2) + width + Box_degree_right)) == 0
    Box_degree_right = Box_degree_right + 1;
end
Box_degree = Box_degree_left;
if Box_degree == 0
    Box_degree = Box_degree_right;
end

