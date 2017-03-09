#include "SFNetworkOscillator.h"

#include <iostream>
#include <vector>
#include <algorithm>
#include <ctime>
#include <functional>
#include <random>
#include <fstream>
#include <thread>
#include <Windows.h>
#include <string>
#include <iomanip>
#include <sstream>
#include <ShObjIdl.h>


#define PI	3.1415926535897932384626433832795
#define O(n, m, k)	n * n * n * m * k


using namespace std;
using namespace Diploma2;



unsigned int split(const string &txt, vector<string> &strs, char ch)
{
	unsigned int pos = txt.find(ch);
	unsigned int initialPos = 0;
	strs.clear();

	// Decompose statement
	while (pos != string::npos) {
		strs.push_back(txt.substr(initialPos, pos - initialPos + 1));
		initialPos = pos + 1;

		pos = txt.find(ch, initialPos);
	}

	// Add the last one
	strs.push_back(txt.substr(initialPos, min(pos, txt.size()) - initialPos + 1));

	return strs.size();
}
double predict(int n, int m, double ts, double ss) {
	const int times = 5;
	SFNetworkOscillator nw = SFNetworkOscillator(n, m, 0, 0, 1, 0, 1, 0, ss, ss);
	double start = clock();
	double N = ts / ss;
	if (N > times) N = times;
	for (int i = 0; i < times; i++)
		nw.SimulateDynamicStep();
	double t = (clock() - start) / times;
	return (t) * ts / ss;
}

void t(char** c) {

}

int main(int argc, char** argv) {
	freopen("queue", "r", stdin);
	vector<string> args(0);
	while (!cin.eof()) {
		args.push_back("");
		getline(cin, args[args.size() - 1]);
	}

	for (string i : args)
	{
		cout << i << endl;
		vector<string> arg;
		int n = split(i, arg, ' ');
		
		vector<char*> cargs(n);
		for (int j = 0; j < n; j++)
			cargs[j] = const_cast<char*>(arg[j].c_str());


	}

	return 0;
}