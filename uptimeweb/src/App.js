import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Dashboard} from './components/Views/Dashboard';
import { SignIn} from './components/Views/SignIn';
import { SignUp } from './components/Views/SignUp';
import { Home } from './components/Views/Home';
import './custom.css'

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/Dashboard' component={Dashboard} />
        <Route exact path='/SignUp' component={SignUp} />
        <Route exact path='/SignIn' component={SignIn} />
        <Route exact path='/' component={Home} />
      </Layout>
    );
  }
}
