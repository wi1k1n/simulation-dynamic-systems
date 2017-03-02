#pragma once
#include <vector>
#include <functional>

//typedef double(*RK4SFunc)(int, double, const std::vector<double>&);
using RK4SFunc = std::function<double(int, double, std::vector<double>)>;

class RK4SResult {
public:
	double time = 0;
	std::vector<double> values;

	RK4SResult(double, const std::vector<double>&);
	~RK4SResult();
};

class RK4S
{
public:
	static RK4SResult Solve(const std::vector<RK4SFunc>&, double, const std::vector<double>&, double, double);
private:
	static void rk_rates_calc(const std::vector<RK4SFunc>&, double, const std::vector<double>&, double, const std::vector<double>&, std::vector<double>&, double);
};
