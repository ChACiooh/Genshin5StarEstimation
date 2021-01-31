# Genshin5StarEstimation
Program which estimates probabilities of pick 5 star characters in n tries on Genshin Impact.

## Developer
- Hanyang University Seoul Korea student whose major is Computer Software Department.
- contact : @7A4ys(twitter) / email:honoki47@gmail.com

## Purpose
People who want to pick new 5 star character always have money issue. So, when people know how much money they have and consider how many they want to pick, this will be helpful.

## Usage
1. input stack value of your gacha (1 ~ 89)
2. input your state whether now is in 100% pick up or not. (y/n)
3. input gachas you can try.

## Output = probability of picking n characters in k tries(k: output %).
They follow CDF model(https://en.wikipedia.org/wiki/Cumulative_distribution_function).
- n_basic.txt : basical probs with s stack you provided.
- n_pick.txt : first gacha gives pick-up character obviously, and probs based on it.
- 1~7.txt : primary information s.t. 0 stack and never tried gacha system(or got pick-up character previous gacha stage). 

## Principle
Previous result of the gacha affects next gacha. And from 76th to 89th tries in each stage of gacha have another probability. Of course, 100% on k >= 90, k is the number of tries. It says Dynamic Programming needed because such system has previous result affects next output. You can consider recursive algorithm but it has the time complexity of O(N^M). However, DP algorithm changes it into O(N*M*K), N and K is very less than M.

## After
The developer made this program as C# language, and will make GUI with WPF system.
