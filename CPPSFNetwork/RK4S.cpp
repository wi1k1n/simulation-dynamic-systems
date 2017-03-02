#include "RK4S.h"

#include <vector>


RK4SResult::RK4SResult(double t, const std::vector<double>& v)
{
	time = t;
	values = v;
}


RK4SResult::~RK4SResult()
{
}



static double sixth = 1.0 / 6.0;
static std::vector<double> koefs = { 0, 0.5, 0.5, 1 };
RK4SResult RK4S::Solve(const std::vector<RK4SFunc>& funcs, double t_init, const std::vector<double>& vals_init, double t_target, double t_step) {
	double t_current = t_init;
	std::vector<double> vals_current(vals_init.size());
	for (int i = 0; i < vals_current.size(); i++)
		vals_current[i] = vals_init[i];
	std::vector<std::vector<double>> rates(5);
	for (int i = 0; i < 5; i++)
		rates[i] = std::vector<double>(vals_init.size());
	double a = sixth;
	while (t_current <= t_target)
	{
		for (int i = 1; i < 5; i++)
			rk_rates_calc(funcs, t_current, vals_current, t_step, rates[i - 1], rates[i], koefs[i - 1]);
		for (int i = 0; i < vals_init.size(); i++) vals_current[i] += sixth * (rates[1][i] + 2 * rates[2][i] + 2 * rates[3][i] + rates[4][i]);
		t_current += t_step;
	}
	return RK4SResult(t_current, vals_current);
}

void RK4S::rk_rates_calc(const std::vector<RK4SFunc>& funcs, double t_current, const std::vector<double>& vals_current, double t_step, const std::vector<double>& rates_prev, std::vector<double>& rates, double k) {
	std::vector<double> vals_new(vals_current.size());
	for (int i = 0; i < vals_current.size(); i++) vals_new[i] = vals_current[i] + k * rates_prev[i];
	double t_new = t_current + k * t_step;
	for (int i = 0; i < vals_current.size(); i++) rates[i] = funcs[i](i, t_new, vals_new) * t_step;
}