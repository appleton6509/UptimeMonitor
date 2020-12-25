import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Dashboard } from './components/Views/Dashboard';
import { SignUp } from './components/Views/SignUp';
import './custom.css'

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/' component={Dashboard} />
        <Route exact path='/SignUp' component={SignUp} />
      </Layout>
    );
  }
}
