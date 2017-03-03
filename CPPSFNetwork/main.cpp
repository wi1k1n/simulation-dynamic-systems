#include "SFNetwork.h"
#include "RK4S.h"
#include "ilRand.h"

#include <iostream>
#include <vector>
#include <algorithm>
#include <ctime>
#include <functional>
#include <random>


#define PI				3.1415926535897932384626433832795


using namespace std;




double measureRuntime(const function<void()> f) {
	clock_t start(clock());
	f();
	return (clock() - start) / (double)CLOCKS_PER_SEC;
}

int main(int argc, char* argv) {
	double start = clock();
	srand(start);
	double runtime = measureRuntime([start]()->void {
		int min = 0, max = 10, seed = 170303, n = 100;
		/*cout << "Enter min:" << endl; cin >> min;
		cout << "Enter max:" << endl; cin >> max;
		cout << "Enter seed:" << endl; cin >> seed;
		cout << "Enter N:" << endl; cin >> n;*/

		ilRand rnd(seed);
		vector<int> vec(n);
		for (int i = 0; i < vec.size(); i++)
			cout << rnd.Next(-50, 50) << endl;
		return;
		SFNetworkOscillator nw(100, 3, .65, 1, 10, -PI, PI, 0, .1, .001, 170302);
		for (int i = 0; i < 100; i++) {
			double local_start = clock();
			nw.SimulateDynamicStep();
			cout << "Time: " << nw.time << "\t" << (clock() - local_start)/1000.0 << "\t" << (clock() - start)/1000.0 << endl;
		}
	});
	cout << endl << "Runtime: " << runtime;
	system("pause");
	return 0;
}