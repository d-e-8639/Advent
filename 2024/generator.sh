

for i in `seq 6 25`
do
    sed s/"TEMPLATE"/"$i"/g AdventTEMPLATE.cs > "Advent$i.cs"
    touch "Advent${i}.txt" "Advent${i}sample.txt"
done
