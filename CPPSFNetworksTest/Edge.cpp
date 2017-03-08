#include "Edge.h"


Edge::Edge() {}

Edge::Edge(int f, int t, int w) {
	from = f;
	to = t;
	weight = w;
}


Edge::~Edge()
{
}
